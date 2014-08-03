using System.ComponentModel.DataAnnotations;

namespace Esthar.Core
{
    public enum FF8TextTagDialog : byte
    {
        [Display(Name = "CardLevel", Description = "Уровень карты")]
        CardLevel = 0x20,
        [Display(Name = "CurrentValue", Description = "Текущее значение")]
        CurrentValue = 0x22,
        [Display(Name = "SelectedGF", Description = "Выбранный GF")]
        SelectedGF = 0x24,
        [Display(Name = "SelectedGFAbility", Description = "Выбранная способность GF")]
        SelectedGFAbility = 0x25,
        [Display(Name = "SelectedMagic", Description = "Выбранная магия")]
        SelectedMagic = 0x26,
        [Display(Name = "SelectedCharacter", Description = "Выбранный персонаж")]
        SelectedCharacter = 0x27,
        [Display(Name = "SelectedParam", Description = "Выбранный параметр")]
        SelectedParam = 0x28,
        [Display(Name = "SelectedBlueMagic", Description = "Выбранная синяя магия")]
        SelectedBlueMagic = 0x29,
    }
}