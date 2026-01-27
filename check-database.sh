#!/bin/bash

# Script to check SQLite database tables

DB_FILE="src/AgroSolutions.Api/AgroSolutions.db"

echo "=== Checking AgroSolutions Database ==="
echo ""

if [ ! -f "$DB_FILE" ]; then
    echo "❌ Database file not found: $DB_FILE"
    echo "   Run 'dotnet run' first to create the database"
    exit 1
fi

echo "✓ Database file exists: $DB_FILE"
echo "  Size: $(ls -lh "$DB_FILE" | awk '{print $5}')"
echo ""

# Check if sqlite3 is available
if ! command -v sqlite3 &> /dev/null; then
    echo "⚠️  sqlite3 command not found. Install it to check tables:"
    echo "   macOS: brew install sqlite3"
    echo "   Linux: sudo apt-get install sqlite3"
    echo ""
    echo "Or use a SQLite browser tool to inspect the database file."
    exit 0
fi

echo "=== Database Tables ==="
sqlite3 "$DB_FILE" ".tables"
echo ""

echo "=== Users Table ==="
sqlite3 "$DB_FILE" "SELECT COUNT(*) as UserCount FROM Users;"
echo ""

echo "=== Users Details ==="
sqlite3 "$DB_FILE" "SELECT Id, Name, Email, Role, IsActive, CreatedAt FROM Users;" -header -column
echo ""

echo "=== Database Schema ==="
sqlite3 "$DB_FILE" ".schema Users"
