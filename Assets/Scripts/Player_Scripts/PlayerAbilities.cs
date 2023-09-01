using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header ("Active Abilities Information")]
    [SerializeField] private List<AbilityTag> activeAbilities;

    private void Awake (){
        activeAbilities = new List<AbilityTag>();
    }

    public void ChangeAbilites (List<AbilityTag> abilityList){
        activeAbilities = abilityList;
    }

    private void SetUpAbilityActivation (List<AbilityTag> newAbilities){
        var setOfPreviousAbilities = new HashSet<AbilityTag>(activeAbilities);

        foreach (var ability in newAbilities)
        {
            if (setOfPreviousAbilities.Contains(ability)) continue;

            switch (ability){
                // TODO
            }
            
        }
    }


    
}

public enum AbilityTag{
    
}
