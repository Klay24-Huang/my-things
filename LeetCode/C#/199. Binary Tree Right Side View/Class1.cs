namespace _199._Binary_Tree_Right_Side_View
{
    public class TreeNode
    {
        public int val;
        public TreeNode left;
        public TreeNode right;
        public TreeNode(int val = 0, TreeNode left = null, TreeNode right = null)
        {
            this.val = val;
            this.left = left;
            this.right = right;
        }
    }
    public class Solution
    {
        public IList<int> RightSideView(TreeNode root)
        {
            var ans = new List<int>();
            var layer = new List<TreeNode>
            {
                root
            };
            if (root is null) { return ans; }

            while (true)
            {
                ans.Add(layer[0].val);

                var newLayer = new List<TreeNode>();
                foreach (var node in layer)
                {
                    if (node.right is not null)
                        newLayer.Add(node.right);

                    if (node.left is not null)
                        newLayer.Add(node.left);
                }
                layer = newLayer;

                if (layer.Count == 0)
                { break; }
            }
            return ans;
        }

        public int LayerRightSideView(List<TreeNode> trees)
        {
            return trees[0].val;
        }
    }
}