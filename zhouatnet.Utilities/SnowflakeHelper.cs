namespace zhouatnet.Utilities
{
    /// <summary>
    /// 雪花帮助类
    /// </summary>
    public class SnowflakeHelper
    {
        private IdWorker _IdWorker;
        public IdWorker IdWorker { get { return this._IdWorker; } }

        private static SnowflakeHelper _Singleton;
        private static readonly object obj = new object();
        private SnowflakeHelper()
        {
            this._IdWorker = new IdWorker(1, 1);
        }

        public static SnowflakeHelper GetInstance()
        {
            if (_Singleton == null)
            {
                //加锁保护，在多线程下可以确保实例值被创建一次。
                //缺点是每 次获取单例，都要进行判断，涉及到的锁和解锁比较耗资源。
                lock (obj)
                {
                    if (_Singleton == null)
                    {
                        _Singleton = new SnowflakeHelper();
                    }
                }

            }
            return _Singleton;
        }
    }
}
