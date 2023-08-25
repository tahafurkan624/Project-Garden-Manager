using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HelmetMaster.Extensions
{
    public static class EventTriggerExtensions
    {
        public static void AddListener(this EventTrigger trigger, EventTriggerType triggerType,
            UnityAction<BaseEventData> action)
        {
            if (trigger.triggers.Any(i => i.eventID == triggerType))
            {
                foreach (var entry in trigger.triggers.Where(entry => entry.eventID == triggerType))
                {
                    entry.callback.AddListener(action);
                    break;
                }
            }
            else
            {
                var entry = new EventTrigger.Entry
                {
                    eventID = triggerType
                };
                entry.callback.AddListener(action);
                trigger.triggers.Add(entry);
            }
        }

        public static void RemoveListener(this EventTrigger trigger, EventTriggerType triggerType,
            UnityAction<BaseEventData> action)
        {
            foreach (var entry in trigger.triggers.Where(entry => entry.eventID == triggerType))
            {
                entry.callback.RemoveListener(action);
                break;
            }
        }

        public static void RemoveAllListeners(this EventTrigger trigger, EventTriggerType triggerType)
        {
            foreach (var entry in trigger.triggers.Where(entry => entry.eventID == triggerType))
            {
                entry.callback.RemoveAllListeners();
                break;
            }
        }
    }
}