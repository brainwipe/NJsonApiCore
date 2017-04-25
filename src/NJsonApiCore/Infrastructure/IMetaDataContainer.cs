using NJsonApi.Infrastructure;

namespace NJsonApi.Infrastructure
{
    public interface IMetaDataContainer
    {
        MetaData GetMetaData();
    }

    public class MetaDataContainer : IMetaDataContainer
    {
        private MetaData _metaData = new MetaData();

        public MetaData GetMetaData() { return _metaData; }
    }
}
