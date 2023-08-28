# Definition for a binary tree node.
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right
class Solution:
    def maxDepth(self, root: Optional[TreeNode]) -> int:
        if root is None:
            return 0
        
        # is end node
        if root.val is not None and root.left is None and root.right is None:
            return 1
        
        return 1 + max(self.maxDepth(root.left), self.maxDepth(root.right))
    
    # def countNode(self, root:Optoinal[TreeNode]) -> int:
    #     if root is None:
    #         return 0
        
    #     # is end node
    #     if root.val is not None and root.left is None and root.right is None:
    #         return 1
        
    #     return 1 + max(self.countNode(self, root.left), self.countNode(self, root.right))
    