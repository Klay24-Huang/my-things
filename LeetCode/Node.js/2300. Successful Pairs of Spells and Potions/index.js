/**
 * @param {number[]} spells
 * @param {number[]} potions
 * @param {number} success
 * @return {number[]}
 */
var successfulPairs = function (spells, potions, success) {
    const ans = []
    potions.sort((a, b) => a - b)
    for (const spell of spells) {
        // console.log('curr spell', spell)
        // console.log('portion', potions)
        // console.log('check', spell * potions[potions.length - 1] < success)
        if (spell * potions[potions.length - 1] < success) {
            ans.push(0)
            continue
        }

        let l = 0
        let r = potions.length
        while (l <= r) {
            let currIndex = Math.floor((l + r) / 2)
            // console.log('current index', currIndex)
            if (potions[currIndex] * spell >= success) {
                if (currIndex === 0 || potions[currIndex - 1] * spell < success) {
                    ans.push(potions.length - currIndex)
                    break
                }
                r = currIndex - 1
            } else {
                l = currIndex + 1
            }
        }
        if (l > r) ans.push(0)
    }
    // console.log('ans', ans)
    return ans
};

export default successfulPairs