using System.ComponentModel.DataAnnotations;

namespace Esthar.Core
{
    public enum FF8TextTagColor : byte
    {
        [Display(Name = "Disabled", Description = "Цвет недоступного элемента")]
        Disabled = 0x20,
        [Display(Name = "Grey", Description = "Серый цвет")]
        Grey,
        [Display(Name = "Yellow", Description = "Жёлтый цвет")]
        Yellow,
        [Display(Name = "Red", Description = "Красный цвет")]
        Red,
        [Display(Name = "Green", Description = "Зелёный цвет")]
        Green,
        [Display(Name = "Blue", Description = "Голубой цвет")]
        Blue,
        [Display(Name = "Purple", Description = "Пурпурный цвет")]
        Purple,
        [Display(Name = "White", Description = "Белый цвет")]
        White,
        [Display(Name = "DisabledBlink", Description = "Мерцающий цвет недоступного элемента")]
        DisabledBlink,
        [Display(Name = "GreyBlink", Description = "Мерцающий серый цвет")]
        GreyBlink,
        [Display(Name = "YellowBlink", Description = "Мерцающий жёлтый цвет")]
        YellowBlink,
        [Display(Name = "RedBlink", Description = "Мерцающий красный цвет")]
        RedBlink,
        [Display(Name = "GreenBlink", Description = "Мерцающий зелёный цвет")]
        GreenBlink,
        [Display(Name = "BlueBlink", Description = "Мерцающий голубой цвет")]
        BlueBlink,
        [Display(Name = "PurpleBlink", Description = "Мерцающий пурпурный цвет")]
        PurpleBlink,
        [Display(Name = "WhiteBlink", Description = "Мерцающий белый цвет")]
        WhiteBlink,
    }
}