<#
=====================================================================
  Running SQL against LocalDB - quick reference + helper
  Database: ShopPractice on (localdb)\MSSQLLocalDB
=====================================================================

  LOAD THE HELPER (once per terminal session):
      . .\sql-practice\Run-Query.ps1

  Then run queries with auto-sized output (each column padded to its
  widest value - easiest to read):
      qt "SELECT * FROM Products ORDER BY Price DESC"
      qt (Get-Content .\sql-practice\myqueries.sql -Raw)   # run the .sql file

---------------------------------------------------------------------
  RAW sqlcmd ALTERNATIVES (no helper needed)
---------------------------------------------------------------------
  Connection flags used everywhere:
      -S "(localdb)\MSSQLLocalDB"   server (the LocalDB instance)
      -d ShopPractice               database
      -E                            trusted (Windows) auth

  1) One-off query, compact output (-W trims widths, -s"|" separates columns):
       sqlcmd -S "(localdb)\MSSQLLocalDB" -d ShopPractice -E -W -s"|" -Q "SELECT * FROM Products;"

  2) Run a script file:
       sqlcmd -S "(localdb)\MSSQLLocalDB" -d ShopPractice -E -W -s"|" -i .\sql-practice\myqueries.sql

  3) Save the output to a file (-o):
       sqlcmd -S "(localdb)\MSSQLLocalDB" -d ShopPractice -E -W -s"|" -i .\sql-practice\myqueries.sql -o .\sql-practice\output.txt

  4) Interactive session (type SQL, put GO on its own line to run, EXIT to quit):
       sqlcmd -S "(localdb)\MSSQLLocalDB" -d ShopPractice -E

  5) Cap character-column width (instead of the full type width):
       sqlcmd -S "(localdb)\MSSQLLocalDB" -d ShopPractice -E -Y 20 -Q "SELECT * FROM Products;"

  Handy sqlcmd flags:
      -W            trim trailing spaces (shrink columns to fit)
      -s "|"        column separator
      -w 250        screen width before wrapping
      -Y 20         cap fixed/char column display width
      -o file.txt   write output to a file
=====================================================================
#>

# Runs a query against ShopPractice and prints the result with each
# column auto-sized to its widest value (via Format-Table -AutoSize).
# Uses ADO.NET, which is built into Windows PowerShell (no module needed).
function qt {
    param(
        [Parameter(Mandatory)]
        [string]$Query
    )

    $connectionString = "Server=(localdb)\MSSQLLocalDB;Database=ShopPractice;Integrated Security=true"
    $connection = New-Object System.Data.SqlClient.SqlConnection $connectionString
    try {
        $connection.Open()
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($Query, $connection)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        $table | Format-Table -AutoSize
    }
    finally {
        $connection.Close()
    }
}

Write-Host "Loaded 'qt' helper. Usage: qt 'SELECT * FROM Products'" -ForegroundColor Green
