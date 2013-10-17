namespace Bocce.Notifications
{
    internal interface IDiagnosticTrace
    {
        void ResourceMiss(Resource resource);
        void ResourceFellBack(Resource resource);
        void ResourceCleared(string resourceType);
        void ResourceHit(Resource resource);
    }
}