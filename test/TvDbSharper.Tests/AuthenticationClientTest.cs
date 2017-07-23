﻿namespace TvDbSharper.Tests
{
    using System;
    using System.Threading.Tasks;

    using TvDbSharper.Clients;
    using TvDbSharper.Clients.Authentication;
    using TvDbSharper.Errors;
    using TvDbSharper.Tests.NewPattern;

    using Xunit;

    public class AuthenticationClientTest
    {
        #region AuthenticateAsync tests

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_Makes_The_Right_Request()
        {
            var authenticationRequest = new AuthenticationData("test1", "test2", "test3");

            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync(authenticationRequest, token))
                .ShouldRequest("POST", "/login", "{\"ApiKey\":\"test1\",\"UserKey\":\"test3\",\"Username\":\"test2\"}")
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_Updates_The_Auth_Token()
        {
            var authenticationRequest = new AuthenticationData("test1", "test2", "test3");

            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync(authenticationRequest, token))
                .AssertThat((client, parser) => Assert.Equal("Bearer auth_token", client.DefaultRequestHeaders["Authorization"]))
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_Without_CT_Makes_The_Right_Request()
        {
            var authenticationRequest = new AuthenticationData("test1", "test2", "test3");

            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync(authenticationRequest))
                .ShouldRequest("POST", "/login", "{\"ApiKey\":\"test1\",\"UserKey\":\"test3\",\"Username\":\"test2\"}")
                .WithNoCancellationToken()
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_With_Plain_Values_Makes_The_Right_Request()
        {
            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync("test1", "test2", "test3", token))
                .ShouldRequest("POST", "/login", "{\"ApiKey\":\"test1\",\"UserKey\":\"test3\",\"Username\":\"test2\"}")
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_With_Plain_Values__With_No_CT_Makes_The_Right_Request()
        {
            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync("test1", "test2", "test3"))
                .ShouldRequest("POST", "/login", "{\"ApiKey\":\"test1\",\"UserKey\":\"test3\",\"Username\":\"test2\"}")
                .WithNoCancellationToken()
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_With_ApiKey_Only_Makes_The_Right_Request()
        {
            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync("test1", token))
                .ShouldRequest("POST", "/login", "{\"ApiKey\":\"test1\"}")
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_With_ApiKey_Only__With_No_CT_Makes_The_Right_Request()
        {
            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync("test1"))
                .ShouldRequest("POST", "/login", "{\"ApiKey\":\"test1\"}")
                .WithNoCancellationToken()
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task AuthenticateAsync_Throws_When_Passed_Null_AuthenticationData()
        {
            return AuthenticateAsyncTest()
                .WhenCallingAMethod((client, token) => client.AuthenticateAsync((AuthenticationData)null, token))
                .ShouldThrow<ArgumentNullException>()
                .RunAsync();
        }

        #endregion

        #region RefreshToken tests

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task RefreshTokenAsync_Makes_The_Right_Request()
        { 
            return RefreshTokenAsyncTest()
                .WhenCallingAMethod((client, token) => client.RefreshTokenAsync(token))
                .ShouldRequest("GET", "/refresh_token")
                .RunAsync();
        }

        [Fact]

        // ReSharper disable once InconsistentNaming
        public Task RefreshTokenAsync_Without_CT_Makes_The_Right_Request()
        {
            return RefreshTokenAsyncTest()
                .WhenCallingAMethod((client, token) => client.RefreshTokenAsync())
                .ShouldRequest("GET", "/refresh_token")
                .WithNoCancellationToken()
                .RunAsync();
        }
        
        #endregion

        private static ApiTest<AuthenticationClient> AuthenticateAsyncTest()
        {
            return new ApiTest<AuthenticationClient>()
                .WithErrorMap(ErrorMessages.Authentication.AuthenticateAsync)
                .WithConstructor((client, parser) => new AuthenticationClient(client, parser))
                .SetApiResponse(new ApiResponse())
                .SetResultObject(new AuthenticationResponse("auth_token"))
                .HasNoReturnValue();
        }

        private static ApiTest<AuthenticationClient> RefreshTokenAsyncTest()
        {
            return new ApiTest<AuthenticationClient>()
                .WithErrorMap(ErrorMessages.Authentication.RefreshTokenAsync)
                .WithConstructor((client, parser) => new AuthenticationClient(client, parser))
                .SetApiResponse(new ApiResponse())
                .SetResultObject(new AuthenticationResponse("auth_token"))
                .HasNoReturnValue();
        }
    }
}