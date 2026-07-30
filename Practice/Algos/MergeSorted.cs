namespace Algos
{
    public static class MergeSortedArrays
    {
        public static int[] mergeSortedArrays(int[] nums1, int[] nums2)
        {
            if(nums1 is null && nums2 is null)
            {
                return new int[0];
            }

            if(nums1 is null)
            {
                return nums2;
            }
            
            if(nums2 is null)
            {
                return nums1;
            }

            int l1 = nums1.Length, l2 = nums2.Length, resL = l1 + l2 - 1;
            int[] result = new int[l1 + l2];
            int i = 0, j = 0, k = 0;

            while (i < l1 && j < l2)
            {
                if(nums1[i] < nums2[j])
                {
                    result[k++] = nums1[i++];
                }
                else
                {
                    result[k++] = nums2[j++];
                }
            }

            while ( i < l1)
            {
                result[k++] = nums1[i++];
            }

            while ( j < l2)
            {
                result[k++] = nums2[j++];
            }

            return result;
        }
    }
}