
namespace Tools
{
    /// <summary>
    /// Интерфейс, для централизованного выполнения Update для всех зарегистрированных объектов.
    /// Обязательно зарегистрировать объект с этим интерфейсом в Updater через Register и Unregister.
    /// </summary>
    public interface INeedUpdate
    {
        /// <summary>
        /// Execute every frame.
        /// </summary>
        public void UpdateMe();
    }
}
