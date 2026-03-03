import { Link } from "react-router-dom"
import type { Game } from "../types/game"

interface GameCardProps {
    game: Game
}

function GameCard({ game }: GameCardProps) {
    const year = game.releaseDate ? game.releaseDate.split("-")[0]: "Unknown"

    return (
        <Link   
            to={`/games/${game.id}`}
            className="flex bg-gray-800 rounded-lg overflow-hidden
                        hover:bg-gray-700 hover:-translate-y-0.5
                        transition-all duration-200" 
        >
            {game.coverArtUrl && (
                <img
                    src={game.coverArtUrl}
                    alt={game.title}
                    className="w-20 h-28 object-cover shrink-0"
                />
            )}
            <div className="p-4 min-w-0">
                <h3 className="text-white font-semibold truncate">{game.title}</h3>
                <p className="text-sm text-gray-400 mt-1">
                    {game.developer ?? "Unknown developer"} | {year}
                </p>
                <p className="text-sm text-gray-400 mt-1">
                    {game.publisher ?? "Unknown publisher"}
                </p>
            </div>
        </Link>

    )
}

export default GameCard
