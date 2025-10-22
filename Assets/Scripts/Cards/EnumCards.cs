enum Symbol
{
    Heart,
    Spades,
    Clubs,
    Diamonds,
    Count
}

enum Value
{
    King,
    Queen,
    Jack,
    Ace,
    Count
}

struct Card
{
    public Value value;
    public Symbol symbol;
} 
