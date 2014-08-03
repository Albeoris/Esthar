using System.ComponentModel.DataAnnotations;

namespace Esthar.Core
{
    public enum FF8TextTagCharacter : byte
    {
        [Display(Name = "Squall", Description = "Имя Скволла")]
        Squall = 0x30,
        [Display(Name = "Zell", Description = "Имя Зелла")]
        Zell = 0x31,
        [Display(Name = "Irvine", Description = "Имя Ирвина")]
        Irvine = 0x32,
        [Display(Name = "Quistis", Description = "Имя Квистис")]
        Quistis = 0x33,
        [Display(Name = "Rinoa", Description = "Имя Риноа")]
        Rinoa = 0x34,
        [Display(Name = "Selphie", Description = "Имя Сельфи")]
        Selphie = 0x35,
        [Display(Name = "Seifer", Description = "Имя Сейфера")]
        Seifer = 0x36,
        [Display(Name = "Edea", Description = "Имя Эдеи")]
        Edea = 0x37,
        [Display(Name = "Laguna", Description = "Имя Лагуны")]
        Laguna = 0x38,
        [Display(Name = "Kiros", Description = "Имя Кироса")]
        Kiros = 0x39,
        [Display(Name = "Ward", Description = "Имя Варда")]
        Ward = 0x3A,
        [Display(Name = "Angelo1", Description = "Имя Анжело?")]
        Angelo1 = 0x3B,
        [Display(Name = "Griever", Description = "Имя Гривера")]
        Griever = 0x3C,
        [Display(Name = "Mog", Description = "Имя Мугла")]
        Mog = 0x3D,
        [Display(Name = "Chocobo", Description = "Имя Чокобо")]
        Chocobo = 0x3E,
        [Display(Name = "UnknownCharacter2", Description = "???")]
        UnknownCharacter2 = 0x3F,
        [Display(Name = "Angelo2", Description = "Имя Анжело?")]
        Angelo2 = 0x40,
        [Display(Name = "UnknownCharacter3", Description = "???")]
        UnknownCharacter3 = 0x50,
        [Display(Name = "UnknownCharacter4", Description = "???")]
        UnknownCharacter4 = 0x60
    }
}