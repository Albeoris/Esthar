using System.ComponentModel.DataAnnotations;

namespace Esthar.Core
{
    public enum FF8TextTagKey : byte
    {
        [Display(Name = "EscapeKey1", Description = "Кнопка побега 1")]
        EscapeKey1 = 0x20,
        [Display(Name = "EscapeKey2", Description = "Кнопка побега 2")]
        EscapeKey2 = 0x21,
        [Display(Name = "Menu", Description = "Меню")]
        Menu = 0x24,
        [Display(Name = "Confirm", Description = "Подтверждение")]
        Confirm = 0x25,
        [Display(Name = "Cancel", Description = "Отмены")]
        Cancel = 0x26,
        [Display(Name = "Cards", Description = "Игра в карты")]
        Cards = 0x27,
        [Display(Name = "Select", Description = "Select")]
        Select = 0x28,
        [Display(Name = "EndConcertKey", Description = "?")]
        EndConcertKey = 0x2B,
        [Display(Name = "LeftKey", Description = "Влево")]
        LeftKey = 0x2C,
        [Display(Name = "RightKey", Description = "Вправо")]
        RightKey = 0x2D,
        [Display(Name = "UpKey", Description = "Вверх")]
        UpKey = 0x2E,
        [Display(Name = "DownKey", Description = "Вниз")]
        DownKey = 0x2F,
        [Display(Name = "L1", Description = "L1")]
        L1 = 0x32,
        [Display(Name = "R1", Description = "R1")]
        R1 = 0x33,
        [Display(Name = "Triangle", Description = "Треугольник")]
        Triangle = 0x34,
        [Display(Name = "Circle", Description = "Круг")]
        Circle = 0x35,
        [Display(Name = "Cross", Description = "Крест")]
        Cross = 0x36,
        [Display(Name = "Square", Description = "Квадрат")]
        Square = 0x37,
        [Display(Name = "DefaultKey", Description = "?")]
        DefaultKey = 0x38,
        [Display(Name = "EndKey", Description = "?")]
        EndKey = 0x3B,
        [Display(Name = "PartyKey", Description = "?")]
        PartyKey = 0x64
    }
}