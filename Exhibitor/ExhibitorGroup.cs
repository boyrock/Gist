using nobnak.Gist.InputDevice;
using nobnak.Gist.Loader;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Exhibitor {

    public class ExhibitorGroup : AbstractExhibitor {

        [SerializeField]
        protected AbstractExhibitor[] exhibitors = new AbstractExhibitor[0];
        [SerializeField]
        protected Data data = new Data();

        protected Validator dataValidator = new Validator();

        protected int selectedTab = 0;
        protected string[] tabNames = new string[0];

        #region Unity
        private void OnEnable() {
            dataValidator.Reset();
            dataValidator.Validation += () => {
                data.UpdateFrom(exhibitors);
                tabNames = exhibitors.Select(v => v.name).ToArray();
            };
        }
        #endregion

        #region interface
        #region Exhibitor
        public override void DeserializeFromJson(string json) {
            JsonUtility.FromJsonOverwrite(json, data);
            data.ApplyTo(exhibitors);
        }
        public override object RawData() {
            dataValidator.Validate();
            return data;
        }
        public override string SerializeToJson() {
            dataValidator.Validate();
            return JsonUtility.ToJson(data, true);
        }
        public override void ApplyViewModelToModel() {
            foreach (var ex in exhibitors)
                ex.ApplyViewModelToModel();
        }
        public override void ResetViewModelFromModel() {
            foreach(var ex in exhibitors)
                ex.ResetViewModelFromModel();
        }
        public override void Draw() {
            dataValidator.Validate();

            GUILayout.BeginVertical();
            GUILayout.Label("", GUI.skin.horizontalSlider);

            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            var ex = SelectedExhibitor;
            if (ex != null){
                ex.Draw();
            }

            GUILayout.EndVertical();
        }
        public override void Invalidate() {
            SelectedExhibitor?.Invalidate();
        }
        #endregion

        public AbstractExhibitor SelectedExhibitor {
            get {
                return (0 <= selectedTab && selectedTab < exhibitors.Length)
                    ? exhibitors[selectedTab] 
                    : null;
            }
        }
        #endregion

        #region classes
        public class Data {
            public string[] exhibitorData = new string[0];

            public static Data CreateFrom(AbstractExhibitor[] exhibitors) {
                return new Data().UpdateFrom(exhibitors);
            }

            public Data UpdateFrom(AbstractExhibitor[] exhibitors) {
                exhibitorData = exhibitors.Select(v => v.SerializeToJson()).ToArray();
                return this;
            }
            public Data ApplyTo(AbstractExhibitor[] exhibitors) {
                for (var i = 0; i < exhibitors.Length && i < exhibitorData.Length; i++) {
                    var j = exhibitorData[i];
                    var e = exhibitors[i];
                    e.DeserializeFromJson(j);
                }
                return this;
			}
		}
        #endregion
    }
}
