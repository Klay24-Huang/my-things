var maxOperations = function (nums, k) {
    let map = new Map()
    let operationCount = 0
    for (item of nums) {
        if (item > k) continue
        let target = k - item
        if (map.has(target)) {
            let targetCount = map.get(target)
            targetCount--
            map.set(target, targetCount)
            if (targetCount === 0) {
                map.delete(target)
            }
            operationCount++
            continue
        }

        let hasItem = map.has(item)
        if (hasItem) {
            map.set(item, map.get(item) + 1)
            continue
        }

        map.set(item, 1)
    }
    return operationCount
};

export default maxOperations