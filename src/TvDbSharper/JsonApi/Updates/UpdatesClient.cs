﻿namespace TvDbSharper.JsonApi.Updates
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using TvDbSharper.JsonApi.Updates.Json;
    using TvDbSharper.JsonClient;
    using TvDbSharper.JsonClient.Exceptions;
    using TvDbSharper.RestClient.Json;

    public class UpdatesClient : BaseClient, IUpdatesClient
    {
        public UpdatesClient(IJsonClient jsonClient)
            : base(jsonClient)
        {
        }

        public async Task<TvDbResponse<Update[]>> GetAsync(DateTime fromTime, CancellationToken cancellationToken)
        {
            try
            {
                string requestUri = $"/updated/query?fromTime={fromTime.ToUnixEpochTime()}";

                return await this.GetAsync<Update[]>(requestUri, cancellationToken);
            }
            catch (TvDbServerException ex)
            {
                string message = this.GetMessage(ex.StatusCode, ErrorMessages.Updates.GetAsync);

                if (message == null)
                {
                    throw;
                }

                throw new TvDbServerException(message, ex.StatusCode, ex);
            }
        }

        public async Task<TvDbResponse<Update[]>> GetAsync(DateTime fromTime, DateTime toTime, CancellationToken cancellationToken)
        {
            try
            {
                string requestUri = $"/updated/query?fromTime={fromTime.ToUnixEpochTime()}&toTime={toTime.ToUnixEpochTime()}";

                return await this.GetAsync<Update[]>(requestUri, cancellationToken);
            }
            catch (TvDbServerException ex)
            {
                string message = this.GetMessage(ex.StatusCode, ErrorMessages.Updates.GetAsync);

                if (message == null)
                {
                    throw;
                }

                throw new TvDbServerException(message, ex.StatusCode, ex);
            }
        }
    }
}