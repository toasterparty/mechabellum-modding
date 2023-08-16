using UnityEngine;

namespace MechabellumModding
{
    public static class RecommendedFormationsGui
    {
        public static void OnGUI()
        {
            if (!ShouldShow)
            {
                return;
            }

            float width = Screen.width*0.09f;
            float height = Screen.height*0.04f;

            float xCenter = Screen.width*0.5f;
            float yCenter = Screen.height*0.9f;

            float xOffset = Screen.width*0.05f;
            float xOffset2 = Screen.width*0.16f;

            var nextRect = new Rect(xCenter + xOffset, yCenter, width, height);
            if (GUI.Button(nextRect, ">"))
            {
                RecommendedFormations.SelectNext();
            }

            var prevRect = new Rect(xCenter - xOffset - width, yCenter, width, height);
            if (GUI.Button(prevRect, "<"))
            {
                RecommendedFormations.SelectPrev();
            }

            var addRect = new Rect(xCenter + xOffset2, yCenter, width, height);
            if (GUI.Button(addRect, "Add Formation"))
            {
                RecommendedFormations.Add();
            }

            var deleteRect = new Rect(xCenter - xOffset2 - width, yCenter, width, height);
            if (GUI.Button(deleteRect, "Delete Formation"))
            {
                RecommendedFormations.Delete();
            }

            var idx = RecommendedFormations.CurrentIndex;
            if (idx > 0)
            {                
                var textStyle = new GUIStyle
                {
                    fontSize = (int)(Screen.height * 0.02f),
                    alignment = TextAnchor.MiddleLeft,
                };
                textStyle.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);


                var labelRect = new Rect(xCenter, yCenter, width, height);
                GUI.Label(labelRect, $"{idx}/{RecommendedFormations.MaxIndex}", textStyle);
            }
        }

        private static bool ShouldShow
        {
            get
            {
                return ModConfig.Data.customRecommendedFormations && (RecommendedFormations.FormPanel?.ChooseRecommendFormPanel?.IsShow() ?? false);
            }
        }
    }
}
