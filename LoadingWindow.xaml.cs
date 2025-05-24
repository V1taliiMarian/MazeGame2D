using System;
using System.Windows;

namespace LabyrinthGame
{
    public partial class LoadingWindow : Window
    {
        public Type LevelPageType { get; private set; }

        public LoadingWindow(Type levelPageType)
        {
            InitializeComponent();
            LevelPageType = levelPageType;
            UpdateLoadingText();
        }

        private void UpdateLoadingText()
        {
            string text;

            if (LevelPageType == typeof(RandomLevelPage))
            {
                text = "Генерація випадкового лабіринту...";
            }
            else if (LevelPageType == typeof(TimeAttackPage))
            {
                text = "Завантаження режиму \"Часова атака\"...";
            }
            else if (LevelPageType == typeof(SelectLevelPage))
            {
                text = "Завантаження меню вибору рівня...";
            }
            else if (LevelPageType == typeof(StorePage))
            {
                text = "Завантаження магазину скінів...";
            }
            else if (LevelPageType == typeof(GameModesInfoPage))
            {
                text = "Завантаження інформаційного меню...";
            }
            else if (LevelPageType == typeof(StatisticsPage))
            {
                text = "Завантаження статистики та нагород...";
            }
            else
            {
                text = "Підготовка до гри...";
            }

            LoadingText.Text = text;
        }
    }
}