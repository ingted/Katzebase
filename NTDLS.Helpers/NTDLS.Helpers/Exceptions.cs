namespace NTDLS.Helpers
{
    /// <summary>
    /// Functions for handling exceptions
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        /// Delegate for ignoring exceptions.
        /// </summary>
        public delegate void TryAndIgnoreProc();

        /// <summary>
        /// Delegate for ignoring exceptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public delegate T TryAndIgnoreProc<T>();

        /// <summary>
        /// Delegate for handling exceptions.
        /// </summary>
        public delegate void OnErrorProc(Exception ex);

        /// <summary>
        /// Delegate for handling exceptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public delegate T OnErrorProc<T>(Exception ex);

        /// <summary>
        /// Recursively gets the exception at the bottom of an exception -> InnerException stack.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static Exception? GetRoot(this Exception? ex)
        {
            if (ex?.InnerException != null)
            {
                return GetRoot(ex.InnerException);
            }
            return ex;
        }

        /// <summary>
        /// Executes the given delegate and ignores any exceptions.
        /// </summary>
        /// <param name="func"></param>
        public static void Ignore(TryAndIgnoreProc func)
        {
            try { func(); } catch { }
        }

        /// <summary>
        /// Executes the given delegate and ignores any exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T? Ignore<T>(TryAndIgnoreProc<T> func)
        {
            try { return func(); } catch { }
            return default;
        }

        /// <summary>
        /// Executes the given delegate and ignores any exceptions.
        /// </summary>
        /// <param name="func">Delegate of primary function to call.</param>
        /// <param name="onError">Delegate of function to call in the case of an exception.</param>
        public static void OnError(TryAndIgnoreProc func, OnErrorProc onError)
        {
            try
            {
                func();
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }

        /// <summary>
        /// Executes the given delegate and ignores any exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">Delegate of primary function to call.</param>
        /// <param name="onError">Delegate of function to call in the case of an exception.</param>
        /// <returns></returns>
        public static T? OnError<T>(TryAndIgnoreProc<T> func, OnErrorProc<T> onError)
        {
            try
            {
                return func();
            }
            catch(Exception ex)
            {
                return onError(ex);
            }
        }
    }
}
