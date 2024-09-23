namespace NTDLS.Helpers
{
    /// <summary>
    /// Functions for dealing with Tasks and async.
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Throws an exception if the task did not end successfully.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowTaskException<T>(Task<T> task)
        {
            if (!task.IsCompletedSuccessfully)
            {
                throw new Exception(task.Exception?.Message ?? "An unknown exception occurred.");
            }
        }

        /// <summary>
        /// Throws an exception if the task did not end successfully.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="exceptionText"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowTaskException<T>(Task<T> task, string exceptionText)
        {
            if (!task.IsCompletedSuccessfully)
            {
                throw new Exception(exceptionText);
            }
        }
    }
}
