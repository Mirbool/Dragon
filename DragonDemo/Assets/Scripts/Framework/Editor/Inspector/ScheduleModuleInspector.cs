using TGame.Schedule;
using UnityEditor;
using UnityEngine;

namespace TGame.Editor.Inspector
{
    /// <summary>
    /// 作者: Teddy
    /// 时间: 2018/03/07
    /// 功能: 
    /// </summary>
    [CustomEditor(typeof(ScheduleModule))]
    public class ScheduleModuleInspector : BaseInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                foreach (var scheduler in TGameFramework.Instance.GetModule<ScheduleModule>().Schedulers)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.IntField("Tick Times", scheduler.Times);
                        EditorGUILayout.IntField("Max Times", scheduler.MaxTimes);
                        EditorGUILayout.FloatField("Interval", scheduler.Interval);
                        EditorGUILayout.FloatField("Next Tick Time", scheduler.NextTime);
                        EditorGUILayout.Toggle("Finished", scheduler.Finished);
                        EditorGUILayout.LabelField("OnTick", scheduler.OnTickMethodFullName);
                    }
                    EditorGUILayout.EndVertical();
                }
            }
        }

        protected override void OnInspectorUpdateInEditor()
        {
            base.OnInspectorUpdateInEditor();
            Repaint();
        }
    }
}