﻿using Geotab.Checkmate;
using Geotab.Checkmate.ObjectModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyGeotabAPIAdapter.Utilities
{
    /// <summary>
    /// Contains methods to assist in working with the <see href="https://geotab.github.io/sdk/software/api/reference/">MyGeotab API</see>.
    /// </summary>
    public static class MyGeotabApiUtility
    {
        const int DefaultFeedResultsLimit = 50000;

        /// <summary>
        /// Returns a batch of data of the specified <see cref="Entity"/> type starting at the specified <c>fromDate</c>.
        /// </summary>
        /// <param name="myGeotabApi">An authenticated MyGeotab <see cref="API"/> object.</param>
        /// <typeparam name="T">The type of <see cref="Entity"/> to be retrieved.</typeparam>
        /// <param name="fromDate">The starting ("seed") date to use when making the first <c>GetFeed</c> call. All new data that has arrived after this date will be returned in this call, up to a maximum of <c>resultsLimit</c> data records. The <see cref="FeedResult{T}"/> returned by the feed method will contain the highest version for subsequent calls.</param>
        /// <param name="resultsLimit">The maximum number of records to return. The default and the maximum value is 50,000 unless otherwise indicated in the <see href="https://geotab.github.io/sdk/software/guides/concepts/#result-limits">MyGeotab SDK documentation</see>.</param>
        /// <returns></returns>
        public static async Task<FeedResult<T>> GetFeedAsync<T>(API myGeotabApi, DateTime? fromDate = null, int? resultsLimit = DefaultFeedResultsLimit) where T : Entity
        {
            // Obtain the type parameter type (for logging purposes).
            Type typeParameterType = typeof(T);

            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(Globals.ConfigurationManager.TimeoutSecondsForMyGeotabTasks));

                Task<FeedResult<T>> feedResultTask = Task.Run(() => myGeotabApi.CallAsync<FeedResult<T>>("GetFeed", typeof(T), new
                {
                    resultsLimit,
                    search = new
                    {
                        fromDate
                    }
                }), cancellationTokenSource.Token);

                return await feedResultTask;
            }
            catch (OperationCanceledException exception)
            {
                throw new MyGeotabConnectionException($"MyGeotab API GetFeedAsync call for type '{typeParameterType.Name}' did not return within the allowed time of {Globals.ConfigurationManager.TimeoutSecondsForMyGeotabTasks} seconds. This may be due to a loss of connectivity with the MyGeotab server.", exception);
            }
            catch (Exception exception)
            {
                // If the exception is related to connectivity, wrap it in a MyGeotabConnectionException. Otherwise, just pass it along.
                if (Globals.ExceptionIsRelatedToMyGeotabConnectivityLoss(exception))
                {
                    throw new MyGeotabConnectionException("An exception occurred while attempting to get data from the Geotab API via CallAsync (GetFeed).", exception);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns a batch of data of the specified <see cref="Entity"/> type starting at the specified <c>fromVersion</c>.
        /// </summary>
        /// <param name="myGeotabApi">An authenticated MyGeotab <see cref="API"/> object.</param>
        /// <typeparam name="T">The type of <see cref="Entity"/> to be retrieved.</typeparam>
        /// <param name="fromVersion">Last retrieved version. All new data that has arrived after this version will be returned in this call, up to a maximum of <c>resultsLimit</c> data records. The <see cref="FeedResult{T}"/> returned by the feed method will contain the highest version for subsequent calls. When starting a new feed, if this value is not provided, the call will return only the <c>toVersion</c> (last version in the system).</param>
        /// <param name="resultsLimit">The maximum number of records to return. The default and the maximum value is 50,000 unless otherwise indicated in the <see href="https://geotab.github.io/sdk/software/guides/concepts/#result-limits">MyGeotab SDK documentation</see>.</param>
        /// <returns></returns>
        public static async Task<FeedResult<T>> GetFeedAsync<T>(API myGeotabApi, long? fromVersion, int? resultsLimit = DefaultFeedResultsLimit) where T : Entity
        {
            // Obtain the type parameter type (for logging purposes).
            Type typeParameterType = typeof(T);

            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(Globals.ConfigurationManager.TimeoutSecondsForMyGeotabTasks));

                Task<FeedResult<T>> feedResultTask = Task.Run(() => myGeotabApi.CallAsync<FeedResult<T>>("GetFeed", typeof(T), new
                {
                    fromVersion,
                    resultsLimit
                }), cancellationTokenSource.Token);

                return await feedResultTask;
            }
            catch (OperationCanceledException exception)
            {
                throw new MyGeotabConnectionException($"MyGeotab API GetFeedAsync call for type '{typeParameterType.Name}' did not return within the allowed time of {Globals.ConfigurationManager.TimeoutSecondsForMyGeotabTasks} seconds. This may be due to a loss of connectivity with the MyGeotab server.", exception);
            }
            catch (Exception exception)
            {
                // If the exception is related to connectivity, wrap it in a MyGeotabConnectionException. Otherwise, just pass it along.
                if (Globals.ExceptionIsRelatedToMyGeotabConnectivityLoss(exception))
                {
                    throw new MyGeotabConnectionException("An exception occurred while attempting to get data from the Geotab API via CallAsync (GetFeed).", exception);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets a list of data of the specified <see cref="Type"/> 
        /// </summary>
        /// <returns>The <see cref="Type"/> or <c>null</c> if not found.</returns>
        public static async Task<IList<T>> GetAsync<T>(API myGeotabApi)
        {
            // Obtain the type parameter type (for logging purposes).
            Type typeParameterType = typeof(T);

            try
            { 
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(Globals.ConfigurationManager.TimeoutSecondsForMyGeotabTasks));

                Task<IList<T>> dataListTask = Task.Run(() => myGeotabApi.CallAsync<IList<T>>("Get", typeof(T)), cancellationTokenSource.Token);

                return await dataListTask;
            }
            catch (OperationCanceledException exception)
            {
                throw new MyGeotabConnectionException($"MyGeotab API GetAsync call for type '{typeParameterType.Name}' did not return within the allowed time of {Globals.ConfigurationManager.TimeoutSecondsForMyGeotabTasks} seconds. This may be due to a loss of connectivity with the MyGeotab server.", exception);
            }
            catch (Exception exception)
            {
                // If the exception is related to connectivity, wrap it in a MyGeotabConnectionException. Otherwise, just pass it along.
                if (Globals.ExceptionIsRelatedToMyGeotabConnectivityLoss(exception))
                {
                    throw new MyGeotabConnectionException("An exception occurred while attempting to get data from the Geotab API via CallAsync (Get).", exception);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
