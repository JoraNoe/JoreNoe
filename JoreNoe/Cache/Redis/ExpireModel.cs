namespace JoreNoe.Cache.Redis
{
    /// <summary>
    /// 缓存有效时间辅助类
    /// </summary>
    public class ExpireModel
    {
        /// <summary>
        /// 长缓存
        /// </summary>
        public const int LongCache = 3600;

        /// <summary>
        /// 快速缓存
        /// </summary>
        public const int FastCache = 300;

        /// <summary>
        /// 热缓存
        /// </summary>
        public const int HotCache = 180;

        /// <summary>
        /// 持久缓存
        /// </summary>
        public const int PersistentCache = 7200;
    }
}
