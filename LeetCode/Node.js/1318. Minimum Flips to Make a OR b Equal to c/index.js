/**
 * @param {number} a
 * @param {number} b
 * @param {number} c
 * @return {number}
 */
var minFlips = function (a, b, c) {
	let count = 0;
	// console.log('max', Math.max(a, b, c))
	for (let i = 0; 1 << i <= Math.max(a, b, c); i++) {
		let countOne = 0;
		let currBit = 1 << i;
		if (currBit & a) countOne++;
		if (currBit & b) countOne++;
		if (currBit & c) {
			// is one
			if (countOne === 0) count++;
		} else {
			// is zero
			count += countOne;
		}
		// console.log('turn', i, count)
	}
	return count;
};

export default minFlips;
