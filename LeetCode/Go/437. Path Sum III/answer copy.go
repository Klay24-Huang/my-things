package q437

// import "fmt"

// type TreeNode struct {
// 	Val   int
// 	Left  *TreeNode
// 	Right *TreeNode
// }

// func pathSum(root *TreeNode, targetSum int) int {
// 	ans := 0
// 	hastSet := make(map[*TreeNode]struct{})
// 	var findTarget func(tempSum int, root *TreeNode)
// 	findTarget = func(tempSum int, root *TreeNode) {
// 		if root == nil {
// 			return
// 		}

// 		// top node
// 		if tempSum == 0 {
// 			if _, exists := hastSet[root]; !exists {
// 				hastSet[root] = struct{}{}
// 			} else {
// 				return
// 			}
// 		}

// 		tempSum += root.Val
// 		fmt.Println(tempSum, root.Val)
// 		// fmt.Println(tempSum)
// 		if tempSum == targetSum {
// 			ans++
// 		}
// 		findTarget(tempSum, root.Left)
// 		findTarget(0, root.Left)
// 		findTarget(tempSum, root.Right)
// 		findTarget(0, root.Right)
// 	}
// 	findTarget(0, root)
// 	return ans
// }
