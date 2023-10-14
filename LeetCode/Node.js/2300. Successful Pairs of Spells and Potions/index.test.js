import successfulPairs from ".";

it("test", () => {
	const tests = [
		{
			title: "case 1",
			input: {
				spells: [5, 1, 3],
				potions: [1, 2, 3, 4, 5],
				success: 7,
			},
			expected: [4, 0, 3],
		},
		{
			title: "case 2",
			input: {
				spells: [3, 1, 2],
				potions: [8, 5, 8],
				success: 16,
			},
			expected: [2, 0, 2],
		},
		{
			title: "case 3",
			input: {
				spells: [15, 8, 19],
				potions: [38, 36, 23],
				success: 328,
			},
			expected: [3, 0, 3],
		},
		{
			title: "case 4",
			input: {
				spells: [39, 34, 6, 35, 18, 24, 40],
				potions: [27, 37, 33, 34, 14, 7, 23, 12, 22, 37],
				success: 43,
			},
			expected: [10, 10, 9, 10, 10, 10, 10],
		},
	];

	for (const test of tests) {
		console.info(test.title);
		try {
			const ans = successfulPairs(
				test.input.spells,
				test.input.potions,
				test.input.success
			);
			expect(ans).toStrictEqual(test.expected);
		} catch (error) {
			throw error;
		}
	}
});

// yarn jest '2300. Successful Pairs of Spells and Potions/index.test.js'
