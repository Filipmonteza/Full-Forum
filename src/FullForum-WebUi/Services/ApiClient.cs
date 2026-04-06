using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FullForum_WebUi.Models.Auth.Requests;
using FullForum_WebUi.Models.Auth.Responses;
using FullForum_WebUi.Models.Category;
using FullForum_WebUi.Models.Comments;
using FullForum_WebUi.Models.Comments.Requests;
using FullForum_WebUi.Models.Threads;
using FullForum_WebUi.Models.Users.Activity;
using FullForum_WebUi.Models.Users.Profile.Request;
using FullForum_WebUi.Services.Common;
using FullForum_WebUi.Services.Exceptions;
using FullForum;

namespace FullForum_WebUi.Services;
    /// <summary>
    /// Client used for communication with the Web API.
    /// Handles authentication headers, API calls, and API error mapping.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStore _tokens;
        
        /// <summary>
        /// Creates a configured HttpClient for the Web API.
        /// Adds the bearer token if the user is authenticated.
        /// </summary>
        private HttpClient Client
        {
            get
            {
                var token = _tokens.AccessToken;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }

                return _httpClient;
            }
        }
        
        /// <summary>
        /// Creates a new ApiClient instance.
        /// </summary>
        public ApiClient(HttpClient httpClient, ITokenStore tokens)
        {
            _httpClient = httpClient;
            _tokens = tokens;
        }
        
        /// <summary>
        /// Logs in a user with email and password.
        /// Returns access/authentication data on success.
        /// </summary>
        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var response = await Client.PostAsJsonAsync(
                "/api/auth/login",
                new LoginRequest(email, password));
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("Not logged in or invalid credentials") { Status = 401 };
           
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
                throw new ApiProblemException("API returned an empty or invalid login response.");

            return result;
        }

        /// <summary>
        /// Registers a new user account.
        /// Throws a mapped API exception if validation fails.
        /// </summary>
        public async Task RegisterAsync(string email, string password, string username)
        {
            var response = await Client.PostAsJsonAsync(
                "/api/auth/register",
                new RegisterRequest(email, password, username)); 
            if (response.IsSuccessStatusCode) return;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Throw(await response.Content.ReadAsStringAsync());
            }
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Gets information about the currently authenticated user.
        /// </summary>
        public async Task<MeResponse> MeAsync()
        {
            var response = await Client.GetAsync("/api/auth/me");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<MeResponse>();
            if (result is null)
                throw new ApiProblemException("API returned an empty or invalid user response.");

            return result;
        }

        /// <summary>
        /// Updates the current user's profile information.
        /// Clears the token if the session is no longer valid.
        /// </summary>
        public async Task UpdateProfileAsync(UpdateUserProfileRequest dto)
        {
            var response = await Client.PutAsJsonAsync("/users/profile", dto);
            if (response.IsSuccessStatusCode) return;
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokens.SetToken(null);
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };
            Throw(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Changes the current user's password.
        /// Clears the token if the session is no longer valid.
        /// </summary>
        public async Task ChangePasswordAsync(ChangePasswordRequest dto)
        {
            var response = await Client.PutAsJsonAsync("/users/password", dto);
            if (response.IsSuccessStatusCode) return;
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokens.SetToken(null);
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };

            Throw(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Gets activity information for a specific user.
        /// Includes threads and comments created by the user.
        /// </summary>
        // Get user activity
        public async Task<UserActivityResponse> GetUserActivityAsync(Guid userId)
        {
            var response = await Client.GetAsync($"/users/{userId}/activity");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserActivityResponse>();
                if (result is null)
                    throw new ApiProblemException("API returned an empty or invalid user activity response.");

                return result;
            }

            Throw(await response.Content.ReadAsStringAsync());
            throw new ApiProblemException("Unexpected error while fetching user activity.");
        }
        
        /// <summary>
        /// Gets all available forum categories.
        /// </summary>
        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var response = await Client.GetAsync("/categories");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokens.SetToken(null);
                throw new ApiProblemException("Session expired. Please log in again.") { Status = 401 };
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? [];
            Throw(await response.Content.ReadAsStringAsync());
            return [];
        }
        
        /// <summary>
        /// Gets a single category by its id.
        /// Clears the token if the session has expired.
        /// </summary>
        public async Task<CategoryDto> GetCategoryAsync(Guid id)
        {
            var response = await Client.GetAsync($"/categories/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokens.SetToken(null);
                throw new ApiProblemException("Session expired. Please log in again.") { Status = 401 };
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CategoryDto>();
                if (result is null)
                    throw new ApiProblemException("API returned an empty or invalid category response.");

                return result;
            }

            Throw(await response.Content.ReadAsStringAsync());
            throw new ApiProblemException("Unexpected error while fetching category.");
        }
        
        /// <summary>
        /// Gets a paged list of threads.
        /// Supports optional category filtering, paging, and sorting.
        /// </summary>
        // VG: sortBy accepts date or comments
        public async Task<PagedThreadsModel> GetThreadsAsync(Guid? categoryId = null, int page = 1, int pageSize = 5, string sortBy = "date", bool descending = true)
        {
            var url = $"/threads?page={page}&pageSize={pageSize}&sortBy={sortBy}&descending={descending}";
            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                url += $"&categoryId={categoryId.Value}";
            }
           
            var response = await Client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _tokens.SetToken(null); 
                throw new ApiProblemException("401 Unauthorized - Token is invalid or expired. Please log in again.") { Status = 401 };
            }
            
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PagedThreadsModel>();
            return result ?? new PagedThreadsModel(new List<ThreadListItemModel>(), 0);
        }
        
        public async Task<ThreadModel> GetThreadAsync(Guid id, bool includeComments = false)
        {
            var url = includeComments
                ? $"/threads/{id}?includeComments=true"
                : $"/threads/{id}";

            var response = await Client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ThreadModel>();
                if (result is null)
                    throw new ApiProblemException("API returned an empty or invalid thread response.");

                return result;
            }

            Throw(await response.Content.ReadAsStringAsync());
            throw new ApiProblemException("Unexpected error while fetching thread.");
        }
        
        /// <summary>
        /// Creates a new thread.
        /// Returns the created thread on success.
        /// </summary>
        public async Task<ThreadModel> CreateThreadAsync(CreateThreadRequest dto)
        {
            var response = await Client.PostAsJsonAsync("/threads", dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var result = await response.Content.ReadFromJsonAsync<ThreadModel>();
                if (result is null)
                    throw new ApiProblemException("API returned an empty or invalid created thread response.");

                return result;
            }

            Throw(await response.Content.ReadAsStringAsync());
            throw new ApiProblemException("Unexpected error while creating thread.");
        }
        
        /// <summary>
        /// Updates an existing thread.
        /// </summary>
        public async Task UpdateThreadAsync(Guid id, UpdateThreadRequest dto)
        {
            var response = await Client.PutAsJsonAsync($"/threads/{id}", dto);
            
            if (response.IsSuccessStatusCode) return;
           
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };
            
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };
            
            Throw(await response.Content.ReadAsStringAsync());
        }
        
        /// <summary>
        /// Deletes a thread by id.
        /// </summary>
        public async Task DeleteThreadAsync(Guid id)
        {
            var response = await Client.DeleteAsync($"/threads/{id}");
            if (response.IsSuccessStatusCode) return;
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("Not logged in or invalid credentials") { Status = 401 };
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("Missing permission to delete thread") { Status = 403 };
            Throw(await response.Content.ReadAsStringAsync());
        }
        
        /// <summary>
        /// Gets all comments for a specific thread.
        /// </summary>
        public async Task<List<CommentModel>> GetThreadCommentsAsync(Guid threadId)
        {
            var response = await Client.GetAsync($"/threads/{threadId}/comments");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CommentModel>>() ?? [];
            }

            Throw(await response.Content.ReadAsStringAsync());
            throw new ApiProblemException("Unexpected error while fetching thread comments.");
        }
        
        /// <summary>
        /// Creates a new comment.
        /// Returns the created comment on success.
        /// </summary>
        public async Task<CommentModel> CreateCommentAsync(CommentCreateDto dto)
        {
            var response = await Client.PostAsJsonAsync("/comments", dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var result = await response.Content.ReadFromJsonAsync<CommentModel>();
                if (result is null)
                    throw new ApiProblemException("API returned an empty or invalid comment response.");

                return result;
            }

            Throw(await response.Content.ReadAsStringAsync());
            throw new ApiProblemException("Unexpected error while creating comment.");
        }
        
        /// <summary>
        /// Updates an existing comment.
        /// /// </summary>
        public async Task UpdateCommentAsync(Guid id, CommentUpdateDto dto)
        {
        
            var response = await Client.PutAsJsonAsync($"/comments/{id}", dto);
        
            if (response.IsSuccessStatusCode) return;
        
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiProblemException("401 Unauthorized") { Status = 401 };
        
            if (response.StatusCode == HttpStatusCode.Forbidden)
            throw new ApiProblemException("403 Forbidden") { Status = 403 };
        
            Throw(await response.Content.ReadAsStringAsync());
            
        }
        
        /// <summary>
        /// Deletes a comment by id.
        /// /// /// </summary>
    
        public async Task DeleteCommentAsync(Guid id)
    
        {
            var response = await Client.DeleteAsync($"/comments/{id}");
            if (response.IsSuccessStatusCode) return;
        
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };
        
            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new ApiProblemException("403 Forbidden") { Status = 403 };
        
            Throw(await response.Content.ReadAsStringAsync());
        }
        
        /// <summary>
        /// Gets all threads created by the currently authenticated user.
        /// </summary>
        public async Task<List<ThreadListItemModel>> GetMyThreadsAsync()
        { 
            var response = await Client.GetAsync("/threads/mine");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ApiProblemException("401 Unauthorized") { Status = 401 };
            
            response.EnsureSuccessStatusCode();
        
            return await response.Content.ReadFromJsonAsync<List<ThreadListItemModel>>() ?? [];
        }
        
        /// <summary>
        /// Parses API error responses and throws a mapped ApiProblemException.
        /// Supports both ProblemDetails-style JSON and plain text responses.
        /// </summary>
        private static void Throw(string jsonOrText)
        {
            try
            {
                var pd = JsonSerializer.Deserialize<ProblemDetailsDto>(
                    jsonOrText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (pd is not null && (!string.IsNullOrWhiteSpace(pd.Title) || !string.IsNullOrWhiteSpace(pd.Detail)))
                {
                    throw new ApiProblemException($"{pd.Title}: {pd.Detail}")
                    {
                        Status = pd.Status ?? 0
                    };
                }
                throw new ApiProblemException(jsonOrText);
            }
            catch (JsonException)
            {
                throw new ApiProblemException(jsonOrText);
            }
        }
    }