﻿namespace TvDbSharper.Clients.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    using TvDbSharper.Clients.Authentication.Json;
    using TvDbSharper.Errors;
    using TvDbSharper.JsonClient;

    public class AuthenticationClient : IAuthenticationClient
    {
        public AuthenticationClient(IJsonClient jsonClient, IErrorMessages errorMessages)
        {
            this.JsonClient = jsonClient;
            this.ErrorMessages = errorMessages;
        }

        private IErrorMessages ErrorMessages { get; }

        private IJsonClient JsonClient { get; }

        public async Task AuthenticateAsync(AuthenticationData authenticationData, CancellationToken cancellationToken)
        {
            if (authenticationData == null)
            {
                throw new ArgumentNullException(nameof(authenticationData));
            }

            try
            {
                var response = await this.JsonClient.PostJsonAsync<AuthenticationResponse>("/login", authenticationData, cancellationToken);

                this.UpdateAuthenticationHeader(response.Token);
            }
            catch (TvDbServerException ex)
            {
                string message = this.GetMessage(ex.StatusCode, this.ErrorMessages.Authentication.AuthenticateAsync);

                if (message == null)
                {
                    throw;
                }

                throw new TvDbServerException(message, ex.StatusCode, ex);
            }
        }

        public async Task AuthenticateAsync(string apiKey, string username, string userKey, CancellationToken cancellationToken)
        {
            await this.AuthenticateAsync(new AuthenticationData(apiKey, username, userKey), cancellationToken);
        }

        public async Task AuthenticateAsync(string apiKey, string username, string userKey)
        {
            await this.AuthenticateAsync(apiKey, username, userKey, CancellationToken.None);
        }

        public Task AuthenticateAsync(string apiKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateAsync(string apiKey)
        {
            throw new NotImplementedException();
        }

        public async Task AuthenticateAsync(AuthenticationData authenticationData)
        {
            await this.AuthenticateAsync(authenticationData, CancellationToken.None);
        }

        public async Task RefreshTokenAsync(CancellationToken cancellationToken)
        {
            try
            {
                var response = await this.JsonClient.GetJsonAsync<AuthenticationResponse>("/refresh_token", cancellationToken);

                this.UpdateAuthenticationHeader(response.Token);
            }
            catch (TvDbServerException ex)
            {
                string message = this.GetMessage(ex.StatusCode, this.ErrorMessages.Authentication.RefreshTokenAsync);

                if (message == null)
                {
                    throw;
                }

                throw new TvDbServerException(message, ex.StatusCode, ex);
            }
        }

        public async Task RefreshTokenAsync()
        {
            await this.RefreshTokenAsync(CancellationToken.None);
        }

        private string GetMessage(HttpStatusCode statusCode, IReadOnlyDictionary<int, string> messagesDictionary)
        {
            if (messagesDictionary.ContainsKey((int)statusCode))
            {
                return messagesDictionary[(int)statusCode];
            }

            return null;
        }

        private void UpdateAuthenticationHeader(string token)
        {
            this.JsonClient.AuthorizationHeader = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}