using System;
using System.Collections.Generic;
using GameAssets.Meta.Quests;

namespace GameAssets.General.Server
{
    [Serializable]
    public sealed class PlayfabQuestsList
    {
        public List<BaseQuest> quests = new();
    }
}
