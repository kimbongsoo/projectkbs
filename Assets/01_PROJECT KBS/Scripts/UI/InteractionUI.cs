using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Gpm.Ui;
using Unity.VisualScripting;
using UnityEngine;

namespace KBS
{
    public class InteractionDataContext
    {
        public IInteractionData Data { get; }
        public IInteractionProvider Provider { get; }
        public string ID => Data.ID;
        public bool ShouldRemoveAfterInteraction => Data is InteractionDropItemData;

        public InteractionDataContext(IInteractionData data, IInteractionProvider provider)
        {
            Data = data;
            Provider = provider;
        }
    }
    public class InteractionUI : UIBase
    {
        [SerializeField] private InfiniteScroll infiniteScroll;

        private List<InteractionDataContext> dataContexts = new();
        private Dictionary<string, InteractionUI_ListItemData> stackedUIMap = new();
        private int currentSelectionIndex = -1;

        private void Awake()
        {
            infiniteScroll.itemPrefab.gameObject.SetActive(false);
        }

        public void AddInteractionData(InteractionDataContext context)
        {
            dataContexts.Add(context);

            string id = context.ID;
            if (stackedUIMap.TryGetValue(id, out var existing))
            {
                //똑같은 ID를 가진 데이터가 이미 존재하는 경우 처리
                existing.stackCount++;
                infiniteScroll.UpdateData(existing);
            }
            else
            {
                var listData = new InteractionUI_ListItemData
                {
                    id = id,
                    icon = context.Data.ActionIcon,
                    message = context.Data.ActionMessage,
                };

                stackedUIMap[id] = listData;
                infiniteScroll.InsertData(listData);

                if (currentSelectionIndex < 0)
                    currentSelectionIndex = 0;
            }
        }

        public void RemoveInteractionData(InteractionDataContext context)
        {
            dataContexts.Remove(context);

            string id = context.ID;
            if (stackedUIMap.TryGetValue(id, out var listData))
            {
                listData.stackCount--;

                if (listData.stackCount <= 0)
                {
                    stackedUIMap.Remove(id);
                    infiniteScroll.RemoveData(listData);

                    var list = infiniteScroll.GetDataList();
                    if (list.Count == 0)
                        currentSelectionIndex = -1;
                    else if (currentSelectionIndex >= list.Count)
                        currentSelectionIndex = list.Count - 1;
                }
                else
                {
                    infiniteScroll.UpdateData(listData);
                }
            }

        }

        public void ClearData()
        {

        }

        public void TryInteract()
        {
            if (currentSelectionIndex < 0)
                return;

            var list = infiniteScroll.GetDataList();
            if (currentSelectionIndex >= list.Count)
                return;

            var selected = list[currentSelectionIndex] as InteractionUI_ListItemData;
            if (selected == null)
                return;

            TrySelectById(selected.id);

            list = infiniteScroll.GetDataList();
            if (list.Count == 0)
            {
                currentSelectionIndex = -1;
            }
            else if (currentSelectionIndex >= list.Count)
            {
                currentSelectionIndex = list.Count - 1;
            }

            if (currentSelectionIndex >= 0 && currentSelectionIndex < list.Count)
            {
                if (list[currentSelectionIndex] is InteractionUI_ListItemData newSelected)
                {
                    // newSelected.isSelected = true;
                    // infiniteScroll.UpdateData(newSelected);
                }
            }
        }

        private void TrySelectById(string id)
        {
            var copiedList = new List<InteractionDataContext>(dataContexts);
            foreach (var context in copiedList)
            {
                if (context.ID == id)
                {
                    context.Provider.Interact(context.Data);

                    if (context.ShouldRemoveAfterInteraction)
                    {
                        RemoveInteractionData(context);
                    }
                }
            }
        }
    }
}
