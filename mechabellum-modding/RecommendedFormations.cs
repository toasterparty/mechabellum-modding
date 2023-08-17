using System;
using System.IO;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;
using GameRiver.Client;
namespace MechabellumModding
{
    public static class RecommendedFormations
    {
        private static readonly List<FormCollect> forms = new();
        private static int currentFormIdx = 0;
        private static int idHighwater = 0;
        private static bool PendingSelectNext = false;
        private static bool PendingSelectPrev = false;
        private static bool PendingAdd = false;
        private static bool PendingDelete = false;
        private static int CachedIndex = -1;
        private static int CachedMaxIndex = -1;

        private static void ResetCache()
        {
            CachedIndex = -1;
            CachedMaxIndex = -1;
        }

        public static void SelectNext()
        {
            PendingSelectNext = true;
        }

        public static void SelectPrev()
        {
            PendingSelectPrev = true;
        }

        public static void Add()
        {
            PendingAdd = true;
        }

        public static void Delete()
        {
            PendingDelete = true;
        }

        public static RecommendFormPanel FormPanel
        {
            get
            {
                return formPanel;
            }
        }

        public static int CurrentIndex
        {
            get
            {
                if (CachedIndex != -1)
                {
                    return CachedIndex;
                }

                var currentForm = GetCurrentForm();
                if (currentForm == null) return -1;

                var idx = 1;

                for (int i = 0; i < forms.Count; i++)
                {
                    if (FormEq(currentForm, forms[i].baseData))
                    {
                        CachedIndex = idx;
                        return idx;
                    }
                    else if(AreFormsCompatible(currentForm, forms[i].baseData))
                    {
                        idx++;
                    }
                }

                return -1;
            }
        }

        public static int MaxIndex
        {
            get
            {
                if (CachedMaxIndex != -1)
                {
                    return CachedMaxIndex;
                }

                var currentForm = GetCurrentForm();
                if (currentForm == null) return 0;

                int count = 0;
                for (int i = 0; i < forms.Count; i++)
                {
                    if (AreFormsCompatible(currentForm, forms[i].baseData))
                    {
                        count++;
                    }
                }

                CachedMaxIndex = count;
                return count;
            }
        }

        public static void Load()
        {
            if (!ModConfig.Data.customRecommendedFormations)
            {
                return;
            }

            LoadCustomForms();
            Harmony.CreateAndPatchAll(typeof(RecommendedFormations));
        }

        public static void Update()
        {
            if (!ModConfig.Data.customRecommendedFormations)
            {
                return;
            }

            if (!MatchClient.IsRunning)
            {
                InDeploy = false;
                FirstDeployFinished = false;
            }

            if (ShouldShowRecButton())
            {
                ShowRecButton();
            }

            if (formPanel != null && formPanel.ChooseRecommendFormPanel != null)
            {
                if (PendingSelectNext)
                {
                    PendingSelectNext = false;
                    SelectNextForm();
                }
                else if (PendingSelectPrev)
                {
                    PendingSelectPrev = false;
                    SelectNextForm(false);
                }
                else if (PendingAdd)
                {
                    PendingAdd = false;
                    SaveCurrentForm();
                }
                else if (PendingDelete)
                {
                    PendingDelete = false;
                    DeleteCurrentForm();
                }
            }

            if (!ModConfig.Data.CustomRecommendedFormationKeyboardShortcuts)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                SelectNextForm();
            }
            else if (Input.GetKeyDown(KeyCode.PageDown))
            {
                SelectNextForm(false);
            }
            else if (Input.GetKeyDown(KeyCode.Insert))
            {
                SaveCurrentForm();
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteCurrentForm();
            }
        }

        private static void ApplyAll()
        {
            var currentForm = GetCurrentForm();
            if (currentForm == null) return;
            
            foreach (var form in forms)
            {
                if (AreFormsCompatible(form.baseData, currentForm))
                {
                    ApplyForm(form);
                }
            }
        }

        private static void RefreshRecPanel()
        {
            if (IsFirstDeploy() && (formPanel?.ChooseRecommendFormPanel?.IsShow() ?? false))
            {
                formPanel.HideRecommendForm();
                formPanel.ShowRecommendForm();
            }
        }

        private static bool ShouldShowRecButton()
        {
            if (!IsFirstDeploy()) return false;
            if (formPanel == null) return false;

            return !formPanel.recommendBtn.IsShow() && !formPanel.chooseRecommendFormPanel.IsShow();
        }

        private static bool InDeploy = false;
        private static bool FirstDeployFinished = false;
        [HarmonyPatch(typeof(MatchClient), nameof(MatchClient.OnEnterDeploy))]
        [HarmonyPostfix]
        private static void OnEnterDeploy()
        {
            MechabellumModding.Log.LogInfo("OnEnterDeploy");
            InDeploy = true;

            if (!FirstDeployFinished)
            {
                SelectNext();
            }
        }

        [HarmonyPatch(typeof(MatchClient), nameof(MatchClient.OnFightStart))]
        [HarmonyPrefix]
        private static void OnFightStart()
        {
            MechabellumModding.Log.LogInfo("OnFightStart");
            InDeploy = false;
            FirstDeployFinished = true;
        }

        private static bool IsFirstDeploy()
        {
            if (!MatchClient.IsRunning)
            {
                return false;
            }

            var uis = GameObject.FindObjectsOfType<MainUI>();
            MainUI ui = null;
            foreach (var _ui in uis) 
            {
                if (_ui.match == null) continue;
                ui = _ui;
                break;
            }
            if (!(ui?.FinishDeploymentButton?.IsShow() ?? false)) return false;

            var client = MatchClient.Current;
            if (client == null) return false;

            if (client.IsWatchMatch() || client.IsWatchMatch())
            {
                return false;
            }

            return InDeploy && !FirstDeployFinished;
        }

        private static bool DictionaryEqual<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1.Count != dict2.Count)
            {
                return false;
            }

            foreach (var kvp in dict1)
            {
                if (!dict2.TryGetValue(kvp.Key, out TValue value) || !EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                {
                    return false;
                }
            }

            return true;
        }

        private static Dictionary<int, int> CalculateUnitCounts(FormBase form)
        {
            var unitCounts = new Dictionary<int, int>();

            if (form == null || form.units == null)
            {
                return unitCounts;
            }

            foreach (var unit in form.units)
            {
                if (!unitCounts.ContainsKey(unit.id))
                {
                    unitCounts[unit.id] = 0;
                }

                unitCounts[unit.id]++;
            }

            return unitCounts;
        }

        private static bool AreFormsCompatible(FormBase a, FormBase b)
        {
            return DictionaryEqual(CalculateUnitCounts(a), CalculateUnitCounts(b));
        }

        private static FormBase GetCurrentForm()
        {
            if (formManager == null)
            {
                MechabellumModding.Log.LogWarning($"null formManager");
                return null;
            }

            formManager.TryGetCurrentForm(out PlayerFormCollect form);
            if (form == null)
            {
                MechabellumModding.Log.LogWarning($"Could not get current formation");
                return null;
            }

            return form.baseData;
        }

        private static bool FormEq(FormBase a, FormBase b)
        {
            if (a.units.Count != b.units.Count)
            {
                return false;
            }

            foreach(var aUnit in a.units)
            {
                // for every unit in a

                // check if it matches any unit in b
                bool match = false;
                foreach (var bUnit in b.units)
                {
                    if (aUnit.id != bUnit.id) continue;
                    if (aUnit.pos_x != bUnit.pos_x) continue;
                    if (aUnit.pos_y != bUnit.pos_y) continue;
                    if (aUnit.direction != bUnit.direction) continue;

                    match = true;
                    break;
                }

                // a single unit in a which does not exist in b means they are not equal
                if (!match) return false;
            }

            // otherwise all units in a are found in b, and they have the same number of units, so they are equal
            return true;
        }

        private static void SelectForm(FormBase form)
        {
            if (formPanel == null)
            {
                MechabellumModding.Log.LogWarning($"null formPanel");
                return;
            }

            var chooseFormPanel = formPanel.ChooseRecommendFormPanel;
            if (chooseFormPanel == null)
            {
                MechabellumModding.Log.LogWarning($"null chooseFormPanel");
                return;
            }

            var plans = chooseFormPanel.plans;
            if (plans == null)
            {
                MechabellumModding.Log.LogWarning($"null plans");
                return;
            }

            var formItems = chooseFormPanel.planGameObjects;
            if (formItems == null)
            {
                MechabellumModding.Log.LogWarning($"null formItems");
                return;
            }

            int planIdx = -1;
            for (int i = 0; i < plans.Count; i++)
            {
                if (plans[i].baseData.id == form.id)
                {
                    planIdx = i;
                    break;
                }
            }

            if (planIdx < 0 || planIdx >= formItems.Count)
            {
                planIdx = 0;
                formItems[planIdx].Refresh(Base2Collect(form));
            }

            chooseFormPanel.OnClickForm(formItems[planIdx].itemBtn);
            RefreshRecPanel();
            ResetCache();
            MechabellumModding.Log.LogWarning($"Applied plan {planIdx} id={form.id}");
        }

        private static void SelectNextForm(bool direction = true)
        {
            if (!IsFirstDeploy()) return;

            var currentForm = GetCurrentForm();
            if (currentForm == null) return;

            for (int idx = 1; idx <= forms.Count; idx++)
            {
                int checkIdx;
                
                if (direction)
                {
                    checkIdx = (currentFormIdx + idx) % forms.Count;
                }
                else
                {
                    checkIdx = (currentFormIdx + (forms.Count - idx)) % forms.Count;
                }

                var form = forms[checkIdx];
                var formBase = form.GetBaseData();

                if (!AreFormsCompatible(formBase, currentForm))
                {
                    continue;
                }

                if (FormEq(formBase, currentForm))
                {
                    continue;
                }

                currentFormIdx = checkIdx;
                ApplyForm(form);
                SelectForm(formBase);
                return;
            }

            MechabellumModding.Log.LogInfo("No other saved forms");
        }

        private static void ApplyForm(FormBase form)
        {
            ApplyForm(Base2Collect(form));
        }

        private static void ApplyForm(FormCollect form)
        {
            var manager = RecommendFormManager.Instance;
            if (manager == null)
            {
                MechabellumModding.Log.LogWarning("Manager is null");
                return;
            }

            var dataInfo = manager.dataInfo;
            if (dataInfo == null)
            {
                MechabellumModding.Log.LogWarning("dataInfo is null");
                return;
            }

            dataInfo.AddCurrentRecommend(form);
            RefreshRecPanel();
            MechabellumModding.Log.LogInfo($"Applied form {form.Id}");
        }

        private static void ClearForms()
        {
            var manager = RecommendFormManager.Instance;
            if (manager == null)
            {
                MechabellumModding.Log.LogWarning("Manager is null");
                return;
            }

            var dataInfo = manager.dataInfo;
            if (dataInfo == null)
            {
                MechabellumModding.Log.LogWarning("dataInfo is null");
                return;
            }
            dataInfo.ClearAllData();

            MechabellumModding.Log.LogInfo("Cleared all Recommended formations");
        }

        private static void ShowRecButton()
        {
            var manager = RecommendFormManager.Instance;
            if (manager == null)
            {
                MechabellumModding.Log.LogWarning("Manager is null");
                return;
            }

            if (formPanel == null)
            {
                MechabellumModding.Log.LogWarning("formPanel is null");
                return;
            }

            formPanel.ShowRecommendBtn();
        }

        private static void SaveCurrentForm()
        {
            if (!IsFirstDeploy()) return;

            var form = GetCurrentForm();
            if (form == null) return;

            if (DuplicateForm(form))
            {
                MechabellumModding.Log.LogWarning("This form is already saved");
                return;
            }

            idHighwater++;
            form.name = idHighwater.ToString();
            form.id = idHighwater;
            form.userid = (ulong) idHighwater;
            form.time = DateTime.Now.ToLongDateString() + ' ' + DateTime.Now.ToLongTimeString();

            LoadCustomForm(form, true);
            ApplyForm(form);
        }

        private static void DeleteCurrentForm()
        {
            if (!IsFirstDeploy()) return;

            var form = MatchingForm(GetCurrentForm());
            if (form == null)
            {
                MechabellumModding.Log.LogWarning("Nothing to delete that matches this formation");
                return;
            }

            var dir = Path.Combine(Helpers.GameFolderPath, "custom_formations");
            Directory.CreateDirectory(dir);
            var filepath = Path.Combine(dir, $"{form.baseData.id}.json");
            
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            var idx = -1;
            for (int i = 0; i < forms.Count; i++)
            {
                if (form.baseData.id == forms[i].baseData.id)
                {
                    idx = i;
                    break;
                }
            }

            if (idx != -1)
            {
                forms.RemoveAt(idx);
            }
            else
            {
                MechabellumModding.Log.LogWarning("Failed to find and remove form from RAM");
            }

            RecommendFormManager.Instance?.dataInfo?.ClearCurrentRecommend();
            RefreshRecPanel();
            ApplyAll();
            ResetCache();
            PendingSelectNext = true;

            MechabellumModding.Log.LogInfo($"Deleted form id={form.baseData.id}");
        }

        private static FormCollect Base2Collect(FormBase form)
        {
            return new FormCollect
            {
                baseData = form,
                Flag = form.flag,
                Group = 0,
                Id = form.id,
            };
        }

        private static FormCollect MatchingForm(FormBase x)
        {
            if (x == null) return null;

            foreach (var form in forms)
            {
                if (FormEq(x, form.baseData))
                {
                    return form;
                }
            }

            return null;
        }

        private static bool DuplicateForm(FormBase x)
        {
            return MatchingForm(x) != null;
        }

        private static void LoadCustomFormCallback(FormBase form)
        {
            LoadCustomForm(form);
        }

        private static void LoadCustomForm(FormBase form, bool saveToDisk = false)
        {
            if (form.id > idHighwater)
            {
                idHighwater = form.id;
            }

            if (DuplicateForm(form))
            {
                return;
            }

            if (saveToDisk)
            {                    
                var dir = Path.Combine(Helpers.GameFolderPath, "custom_formations");
                Directory.CreateDirectory(dir);

                var filepath = Path.Combine(dir, $"{form.id}.json");
                RecommendFormManager.Instance?.store?.SaveObjToJson(filepath, form);
                MechabellumModding.Log.LogInfo($"Saved to {filepath}");
            }

            forms.Add(Base2Collect(form));
            ResetCache();
        }

        private static void LoadCustomForms()
        {
            var manager = RecommendFormManager.Instance;
            if (manager == null)
            {
                MechabellumModding.Log.LogWarning("Manager is null");
                return;
            }

            var dataStore = manager.store;
            if (dataStore == null)
            {
                MechabellumModding.Log.LogWarning("dataStore is null");
                return;
            }

            int count = 0;

            try
            {
                var customFormationsDir = Path.Combine(Helpers.GameFolderPath, "custom_formations");
                Directory.CreateDirectory(customFormationsDir);

                string[] files = Directory.GetFiles(customFormationsDir);

                foreach (string filePath in files)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(filePath);
                        dataStore.LoadJsonToObj<FormBase>(fileContent, new Action<FormBase>(LoadCustomFormCallback));
                        count++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading custom formation file: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MechabellumModding.Log.LogError($"Error accessing custom formations: {ex.Message}");
            }

            MechabellumModding.Log.LogInfo($"Loaded {count} custom formations");
        }

        [HarmonyPatch(typeof(RecommendFormPanel), nameof(RecommendFormPanel.Init))]
        [HarmonyPrefix]
        private static void RecommendFormPanel_Init_Prefix(ref RecommendFormPanel __instance)
        {
            MechabellumModding.Log.LogInfo("RecommendFormPanel_Init");
            ClearForms();
        }

        private static RecommendFormPanel formPanel = null;
        [HarmonyPatch(typeof(RecommendFormPanel), nameof(RecommendFormPanel.Init))]
        [HarmonyPostfix]
        private static void RecommendFormPanel_Init(ref RecommendFormPanel __instance)
        {
            formPanel = __instance;
            ApplyAll();
        }

        [HarmonyPatch(typeof(ChooseRecommendFormPanel), nameof(ChooseRecommendFormPanel.Show))]
        [HarmonyPostfix]
        private static void ChooseRecommendFormPanel_Show(ref ChooseRecommendFormPanel __instance)
        {
            foreach (var obj in __instance.planGameObjects)
            {
                obj.Hide();
            }
        }

        private static RecommendFormManager formManager = null;
        [HarmonyPatch(typeof(RecommendFormManager), nameof(RecommendFormManager.Init))]
        [HarmonyPostfix]
        private static void RecommendFormManager_Init(RecommendFormManager __instance)
        {
            MechabellumModding.Log.LogInfo("RecommendFormManager_Init");
            formManager = __instance;
            ClearForms();
        }
    }
}
