using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LabyrinthGame
{
    public class ColorStoreService
    {
        private readonly CoinManager _coinManager;
        private readonly bool _isWallColorService;
        private const string DefaultPlayerColor = "Білий";
        private const string DefaultWallColor = "Сірий";

        public ColorStoreService(CoinManager coinManager, bool isWallColorService = false)
        {
            _coinManager = coinManager;
            _isWallColorService = isWallColorService;

            if (isWallColorService && string.IsNullOrEmpty(_coinManager.LastSelectedWallColor))
            {
                _coinManager.LastSelectedWallColor = DefaultWallColor;
                _coinManager.SaveLastSelectedWallColor();
            }
            else if (!isWallColorService && string.IsNullOrEmpty(_coinManager.LastSelectedColor))
            {
                _coinManager.LastSelectedColor = DefaultPlayerColor;
                _coinManager.SaveLastSelectedColor();
            }
        }

        public IEnumerable<ColorInfo> GetAllColors()
        {
            if (_isWallColorService)
            {
                return ColorManager.WallColorMap.Select(pair =>
                {
                    var name = pair.Key;
                    var color = pair.Value.Fill;
                    var price = ColorManager.WallColorPrices[name];
                    var isPurchased = name == DefaultWallColor ||
                                     _coinManager.PurchasedWallColors.Contains(name) ||
                                     price == 0;
                    return new ColorInfo(name, color, price, isPurchased);
                });
            }
            else
            {
                return ColorManager.PlayerColorMap.Select(pair =>
                {
                    var name = pair.Key;
                    var color = pair.Value;
                    var price = ColorManager.PlayerColorPrices[name];
                    var isPurchased = name == DefaultPlayerColor ||
                                     _coinManager.PurchasedSkins.Contains(name) ||
                                     price == 0;
                    return new ColorInfo(name, color, price, isPurchased);
                });
            }
        }

        public bool CanAfford(string colorName)
        {
            if ((_isWallColorService && colorName == DefaultWallColor) ||
                (!_isWallColorService && colorName == DefaultPlayerColor))
            {
                return true;
            }

            var price = _isWallColorService
                ? ColorManager.WallColorPrices[colorName]
                : ColorManager.PlayerColorPrices[colorName];
            return _coinManager.CanAfford(price);
        }

        public bool IsPurchased(string colorName)
        {
            if (_isWallColorService)
            {
                return colorName == DefaultWallColor ||
                       _coinManager.PurchasedWallColors.Contains(colorName) ||
                       ColorManager.WallColorPrices[colorName] == 0;
            }
            else
            {
                return colorName == DefaultPlayerColor ||
                       _coinManager.PurchasedSkins.Contains(colorName) ||
                       ColorManager.PlayerColorPrices[colorName] == 0;
            }
        }

        public bool TryPurchase(string colorName)
        {
            if ((_isWallColorService && colorName == DefaultWallColor) ||
                (!_isWallColorService && colorName == DefaultPlayerColor))
            {
                SelectColor(colorName);
                return true;
            }

            var price = _isWallColorService
                ? ColorManager.WallColorPrices[colorName]
                : ColorManager.PlayerColorPrices[colorName];

            if (_coinManager.TryPurchase(price))
            {
                if (_isWallColorService)
                {
                    if (!_coinManager.PurchasedWallColors.Contains(colorName))
                    {
                        _coinManager.PurchasedWallColors.Add(colorName);
                        _coinManager.SavePurchasedWallColors();
                    }
                }
                else
                {
                    if (!_coinManager.PurchasedSkins.Contains(colorName))
                    {
                        _coinManager.PurchasedSkins.Add(colorName);
                        _coinManager.SavePurchasedSkins();
                    }
                }
                SelectColor(colorName);
                return true;
            }
            return false;
        }

        public void SelectColor(string colorName)
        {
            if (_isWallColorService)
            {
                _coinManager.LastSelectedWallColor = colorName;
                _coinManager.SaveLastSelectedWallColor();
            }
            else
            {
                _coinManager.LastSelectedColor = colorName;
                _coinManager.SaveLastSelectedColor();
            }
        }

        public string GetSelectedColorName()
        {
            if (_isWallColorService)
            {
                return string.IsNullOrEmpty(_coinManager.LastSelectedWallColor)
                    ? DefaultWallColor
                    : _coinManager.LastSelectedWallColor;
            }
            else
            {
                return string.IsNullOrEmpty(_coinManager.LastSelectedColor)
                    ? DefaultPlayerColor
                    : _coinManager.LastSelectedColor;
            }
        }
    }
}