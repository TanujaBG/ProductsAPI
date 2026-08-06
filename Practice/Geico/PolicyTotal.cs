namespace Practice.Geico
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string PolicyId { get; set; }
        public long Amount { get; set; }
    }

    public class PolicyTotal
    {
        public Dictionary<string, long> CalculatePolicyTotals(Transaction[] transactions)
        {
            var res = new Dictionary<string, long>(StringComparer.Ordinal);
            if (transactions == null)
               return res;

            Dictionary<string, (string PolicyId, long Amount)> seen = new(StringComparer.Ordinal);
            foreach(var transaction in transactions)
            {
                if (seen.TryGetValue(transaction.TransactionId, out var existing))
                {
                   if (existing.PolicyId == transaction.PolicyId && existing.Amount == transaction.Amount)
                    {
                        continue;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Conflicting transaction detected for TransactionId: {transaction.TransactionId}");
                    }
                }
                seen[transaction.TransactionId] = (transaction.PolicyId, transaction.Amount);
                res.TryGetValue(transaction.PolicyId, out var currentTotal);
                res[transaction.PolicyId] = currentTotal + transaction.Amount;
            }   

            return res;
        }
    }

    public class PolicyTotalTest
    {
        public void RunTests()
        {
            var testCases = new (Transaction[] transactions, Dictionary<string, long>? expected, bool shouldThrow)[]
            {
                // valid test
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T2", PolicyId = "P1", Amount = 200 },
                    new Transaction { TransactionId = "T3", PolicyId = "P2", Amount = 300 }
                },
                new()
                {
                    { "P1", 300 },
                    { "P2", 300 }
                }, false),

                

                // valid test with duplicate transaction
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 }
                },
                new()
                {
                    { "P1", 100 }
                }, false),

                // valid test with multiple policies and duplicate transactions 
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T2", PolicyId = "P2", Amount = 200 },
                    new Transaction { TransactionId = "T2", PolicyId = "P2", Amount = 200 }
                },
                new()
                {
                    { "P1", 100 },
                    { "P2", 200 }
                }, false),

                // valid test with multiple transcations with same policy and including duplicates
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T2", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T2", PolicyId = "P1", Amount = 100 }
                },
                new()
                {
                    { "P1", 200 }
                }, false),

                // valid test with multiple transactions with different policies and including duplicates and negative amounts
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T2", PolicyId = "P2", Amount = 200 },
                    new Transaction { TransactionId = "T2", PolicyId = "P2", Amount = 200 },
                    new Transaction { TransactionId = "T3", PolicyId = "P1", Amount = -50 }
                },
                new()
                {
                    { "P1", 50 },
                    { "P2", 200 }
                }, false),

                // very large value of amount for a single policy
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 1_000_000_000L },
                    new Transaction { TransactionId = "T2", PolicyId = "P1", Amount = 1_000_000_000L }
                },
                new()
                {
                    { "P1", 2_000_000_000L }
                }, false),

                // very large value of amount for multiple policies
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 1_000_000_000L },
                    new Transaction { TransactionId = "T2", PolicyId = "P2", Amount = 1_000_000_000L }
                },
                new()
                {
                    { "P1", 1_000_000_000L },
                    { "P2", 1_000_000_000L }
                }, false),

                // empty transactions
                (new Transaction[] { },
                new()
                {
                }, false),

                // conflicting transaction with different policy. 
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T1", PolicyId = "P2", Amount = 100 }
                },
                null, true),

                // conflicting transaction with different amount
                (new []
                {
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                    new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 200 }
                },
                null, true),

                // Duplicate appears after unrelated records
                (
                    new[]
                    {
                        new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                        new Transaction { TransactionId = "T2", PolicyId = "P2", Amount = 200 },
                        new Transaction { TransactionId = "T3", PolicyId = "P1", Amount = 50 },
                        new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 }
                    },
                    new()
                    {
                        { "P1", 150 },
                        { "P2", 200 }
                    },
                    false
                ),

                // Both policy and amount conflict
                (
                    new[]
                    {
                        new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                        new Transaction { TransactionId = "T1", PolicyId = "P2", Amount = 200 }
                    },
                    null,
                    true
                ),

                // IDs are case-sensitive
                (
                    new[]
                    {
                        new Transaction { TransactionId = "T1", PolicyId = "P1", Amount = 100 },
                        new Transaction { TransactionId = "t1", PolicyId = "P1", Amount = 200 }
                    },
                    new()
                    {
                        { "P1", 300 }
                    },
                    false
                )

            };

            foreach (var testCase in testCases)
            {
               try
               {
                   var res = new PolicyTotal().CalculatePolicyTotals(testCase.Item1);
                   // compare the result with the expected value
                   for (int i = 0; i < testCase.Item2.Count; i++)
                   {
                       var key = testCase.Item2.Keys.ElementAt(i);
                       var expectedValue = testCase.Item2[key];
                       var actualValue = res.ContainsKey(key) ? res[key] : 0;
                       if (expectedValue != actualValue)
                       {
                           Console.WriteLine($"Mismatch for policy {key}: expected {expectedValue}, got {actualValue}");
                           Console.WriteLine($"Test case failed for policy {key}");
                           break;
                       }
                   }
                   Console.WriteLine("Test case passed.");
               }
               catch (InvalidOperationException ex)
                {
                    // check if exception is expected for this test case
                    if(testCase.Item3)
                    {
                        Console.WriteLine("Test case passed. Expected exception thrown.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Test case failed, Unexpected exception thrown: {ex.Message}");
                        break;
                    }
                    
                }
               catch (Exception ex)
               {
                Console.WriteLine($"Test case failed,Unexpected exception thrown: {ex.Message}");
               }
            }
        }
    }

}