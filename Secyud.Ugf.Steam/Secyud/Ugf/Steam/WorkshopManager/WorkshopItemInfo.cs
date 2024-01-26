using Steamworks;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemInfo
    {
        public PublishedFileId_t Id { get; }
        public string LocalPath { get; private set; }
        public string Description { get; set; }
        public bool Available { get; private set; }
        public WorkshopConfigInfo ConfigInfo { get; private set; } 
        public WorkshopItemInfo(PublishedFileId_t id)
        {
            Id = id;
        }

        public void RefreshConfigInfo()
        {
            if (SteamUGC.GetItemInstallInfo(Id,
                    out ulong _, out string localPath,
                    260, out uint _))
            {
                
                LocalPath = localPath;
                ConfigInfo = WorkshopConfigInfo.ReadFromLocal(localPath);
                Available = ConfigInfo is not null;
                if (ConfigInfo is not null)
                {
                    ConfigInfo.FieldId = Id.m_PublishedFileId;
                }
            }
        }
    }
}