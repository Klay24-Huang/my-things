var moveZeroes = function (nums) {
    if (nums.length <= 1) return nums
    var zeroPointer

    for (let index = 0; index < nums.length; index++) {
        const num = nums[index];

        if (num === 0 && zeroPointer === undefined) zeroPointer = index
        if (num === 0 && index < zeroPointer) zeroPointer = index
        if (num !== 0 && zeroPointer !== undefined) {
            nums[zeroPointer] = nums[index]
            nums[index] = 0
            zeroPointer = index
            console.log(nums)
        }
    }
    return nums
};

export default moveZeroes