using System.ComponentModel.DataAnnotations;

namespace Esthar.Core
{
    public enum FF8TextTagTerm : byte
    {
        [Display(Name = "Galbadia", Description = "Название страны \"Гальбадия\"")]
        Galbadia = 0x20,
        [Display(Name = "Esthar", Description = "Название страны \"Эстара\"")]
        Esthar,
        [Display(Name = "Balamb", Description = "Название города \"Баламб\"")]
        Balamb,
        [Display(Name = "Dollet", Description = "Название герцогства \"Доллет\"")]
        Dollet,
        [Display(Name = "Timber", Description = "Название города \"Тимбер\"")]
        Timber,
        [Display(Name = "Trabia", Description = "Название территории \"Трабия\"")]
        Trabia,
        [Display(Name = "Centra", Description = "Название континента \"Сентра\"")]
        Centra,
        [Display(Name = "FH", Description = "Название города \"Горизонт Рыбаков\"")]
        FishermansHorizon,
        [Display(Name = "EastAcademy", Description = "Название сада \"Гальбадия\"")]
        EastAcademy,
        [Display(Name = "DesertPrison", Description = "Название пустынной тюрьмы")]
        DesertPrison,
        [Display(Name = "TrabiaGarden", Description = "Название сада \"Трабия\"")]
        TrabiaGarden,
        [Display(Name = "LunarBase", Description = "Название лунной базы")]
        LunarBase,
        [Display(Name = "ShumiVillage", Description = "Название деревни Шуми")]
        ShumiVillage,
        [Display(Name = "DelingCity", Description = "Название города \"Деллинга\"")]
        DelingCity,
        [Display(Name = "BalambGarden", Description = "Название сада \"Баламб\"")]
        BalambGarden,
        [Display(Name = "EastAcademyStation", Description = "Название ж/д станции \"Восточная академия\"")]
        EastAcademyStation,
        [Display(Name = "DolletStation", Description = "Название ж/д станции Доллета")]
        DolletStation,
        [Display(Name = "DesertPrisonStation", Description = "Название ж/д станции пустынной тюрьмы")]
        DesertPrisonStation,
        [Display(Name = "LunarGate", Description = "Название лунных врат")]
        LunarGate,

        [Display(Name = "Restores", Description = "???")]
        Restores,
        [Display(Name = "Status", Description = "Термин: Состояние")]
        Status,
        [Display(Name = "Learns", Description = "???")]
        Learns,
        [Display(Name = "Ability", Description = "Термин: Способность")]
        Ability,
        [Display(Name = "Magic", Description = "Термин: Магия")]
        Magic,
        [Display(Name = "Refine", Description = "Термин: Слияние")]
        Refine,
        [Display(Name = "Junctions", Description = "Термин: Привязка")]
        Junctions,
        [Display(Name = "Raises", Description = "???")]
        Raises,
        [Display(Name = "Command", Description = "Термин: Команда")]
        Command,

        [Display(Name = "Magazine", Description = "Термин: Магазин")]
        Magazine,
        [Display(Name = "UltimeciaCastle", Description = "Название замка Ультимеции")]
        UltimeciaCastle,
        [Display(Name = "Garden", Description = "Название садов")]
        Garden,
        [Display(Name = "Deling", Description = "Название Делинга")]
        Deling,
    }
}