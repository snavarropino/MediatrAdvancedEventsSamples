namespace Publisher
{
    /// <summary>
    /// Strategy to use when publishing events
    /// </summary>
    public enum PublishStrategy
    {
        /// <summary>
        /// Run each notification handler after one another. Returns when all handlers are finished or an exception has been thrown. In case of an exception, any handlers after that will not be run.
        /// Handler execution order: same order than IoC registration order
        /// </summary>
        SyncStopOnException = 1,

        /// <summary>
        /// Run each notification handler in parallel on it's own thread using Task.Run(). Returns after all handlers are processed.  Any exception can be captured by publish caller
        /// Handler execution order:not guarantied
        /// </summary>
        ParallelWait = 2,

        /// <summary>
        /// Run each notification handler on it's own thread using Task.Run(). Returns immediately and does not wait for any handlers to finish. Note that you cannot capture any exceptions, even if you await the call to Publish.
        /// Handler execution order:not guarantied
        /// </summary>
        ParallelNoWait = 3

    }
}