using System.ComponentModel.DataAnnotations;

namespace Esthar.Core
{
    public enum FF8TextTagCode
    {
        // Без параметров
        End = 0x00,
        
        [Display(Name = "Next", Description = "Переход на следующую страницу")]
        Next = 0x01,
        Line = 0x02,
        [Display(Name = "Speaker", Description = "Имя говорящего")]
        Speaker = 0x12,

        // С параметром (байт)
        [Display(Name = "Var", Description = "Значениен игровой переменной")]
        Var = 0x04,
        [Display(Name = "Pause", Description = "Задержка при выводе текста")]
        Pause = 0x09,

        // С параметром (именованый)
        [Display(Name = "Char", Description = "Имя персонажа")]
        Char = 0x03,
        [Display(Name = "Key", Description = "Кнопка управления")]
        Key = 0x05,
        [Display(Name = "Color", Description = "Цвет выводимого текста")]
        Color = 0x06,
        [Display(Name = "Dialog", Description = "Значение диалогового окна")]
        Dialog = 0x0A,
        [Display(Name = "Term", Description = "Название локации")]
        Term = 0x0E,
    }
}