using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Assets.Editor
{

    public enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

    public enum AndroidViewType
    {
        Landscape, Portrait,
        All
    }

    public class ViewSize
    {
        public string Name;
        public int Width;
        public int Height;
    }

    public static class EditorUtils
    {
        static object _gameViewSizesInstance;
        static MethodInfo _getGroup;

        public static AndroidViewType AndroidViewType;

        static EditorUtils()
        {
            // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
            var sizesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            _getGroup = sizesType.GetMethod("GetGroup");
            _gameViewSizesInstance = instanceProp.GetValue(null, null);
        }

        public static List<ViewSize> ViewSizes
        {
            set { }
            get
            {
                return GetViewSizes(GameViewSizeGroupType.Android);
            }
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            var asm = typeof(UnityEditor.Editor).Assembly;
            var sizesType = asm.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            var getGroup = sizesType.GetMethod("GetGroup");
            var instance = instanceProp.GetValue(null, null);
            var group = getGroup.Invoke(instance, new object[] { (int)sizeGroupType });
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
            var gvsType = asm.GetType("UnityEditor.GameViewSize");
            var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            addCustomSize.Invoke(group, new object[] { newSize });
        }

        public static List<ViewSize> GetViewSizes(GameViewSizeGroupType sizeGroupType)
        {
            var viewSizes = new List<ViewSize>();

            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];

            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];

            var iterations = (sizesCount > displayTexts.Length) ? sizesCount : displayTexts.Length;

            for (var i = 0; i < iterations; i++)
            {
                indexValue[0] = i;

                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);

                if (AndroidViewType == AndroidViewType.Landscape && sizeWidth > sizeHeight && sizeWidth > 280)
                    viewSizes.Add(new ViewSize
                    {
                        Name = displayTexts[i],
                        Width = sizeWidth,
                        Height = sizeHeight
                    });
                else if (AndroidViewType == AndroidViewType.Portrait && sizeWidth < sizeHeight)
                    viewSizes.Add(new ViewSize
                    {
                        Name = displayTexts[i],
                        Width = sizeWidth,
                        Height = sizeHeight
                    });
                else if (AndroidViewType == AndroidViewType.All)
                    viewSizes.Add(new ViewSize
                    {
                        Name = displayTexts[i],
                        Width = sizeWidth,
                        Height = sizeHeight
                    });
            }

            viewSizes = viewSizes.OrderBy(x => x.Width).ToList();

            return viewSizes;
        }

        public static void SetSizeToScreen(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            int idx = FindSize(sizeGroupType, width, height);
            if (idx != -1)
                SetSize(idx);
        }

        public static void SetSize(int index)
        {
            var gvWndType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gvWnd = EditorWindow.GetWindow(gvWndType);
            selectedSizeIndexProp.SetValue(gvWnd, index, null);
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            // goal:
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
            // iterate through the sizes via group.GetGameViewSize(int index)

            var group = GetGroup(sizeGroupType);
            var groupType = GetGroup(sizeGroupType).GetType();

            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");

            var sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);

            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];

            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }
            return -1;
        }

        static object GetGroup(GameViewSizeGroupType type)
        {
            return _getGroup.Invoke(_gameViewSizesInstance, new object[] { (int)type });
        }
    }
}