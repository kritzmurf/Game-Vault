import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { getGameById } from "../services/api"
import type { Game } from "../types/game"
import LoadingThrobber from "../components/LoadingThrobber"
import ErrorMessage from "../components/ErrorMessage"
import { formatPlatformName } from "../utils/formatPlatformName"

function GamePage() {
    const { id } = useParams()
    const [game, setGame] = useState<Game | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState("")

    useEffect(() => {
        if (!id) return
        setLoading(true)
        setError("")
        getGameById(Number(id))
            .then(setGame)
            .catch(() => setError("Failed to load game"))
            .finally(() => setLoading(false))
    }, [id])

    if (loading) return <LoadingThrobber />
    if (error) return <ErrorMessage message={error} onRetry={() => window.location.reload()} />
    if (!game) return null

    return (
        <main className="max-w-4xl mx-auto px-4 sm:px-6 py-8">
            <Link
                to={`/platform/${game.platform}`}
                className="text-amber-500 hover:text-amber-400 text-sm"
            >
                ← Back to {formatPlatformName(game.platform)}
            </Link>
            <div className="mt-6 flex flex-col sm:flex-row gap-8">
                {game.coverArtUrl && (
                    <img
                        src={game.coverArtUrl}
                        alt={game.title}
                        className="w-48 h-auto rounded-lg shrink-0 self-center sm:self-start"
                    />
                )}
                <div>
                    <h1 className="text-2xl sm:text-3xl font-bold">{game.title}</h1>
                    <dl className="mt-4 space-y-2 text-sm">
                        <div>   <dt className="text-gray-500 inline">Developer:</dt>
                                <dd className="inline">{game.developer ?? "Unknown"}</dd> 
                        </div>
                        <div>   <dt className="text-gray-500 inline">Publisher:</dt>
                                <dd className="inline">{game.publisher ?? "Unknown"}</dd> 
                        </div>
                        <div>   <dt className="text-gray-500 inline">Released:</dt>
                                <dd className="inline">{game.releaseDate ?? "Unknown"}</dd> 
                        </div>
                        {/*
                        <div>   <dt className="text-gray-500 inline">Region:</dt>
                                <dd className="inline">{game.region ?? "Unknown"}</dd> 
                        </div>
                        */}
                    </dl>
                </div>
            </div>
            {game.description && (
                <div className="mt-8">
                    <h2 className="text-xl font-semibold mb-3">Description</h2>
                    <p className="text-gray-300 leading-relaxed">{game.description}</p>
                </div>
            )}
        </main>
    )
}

export default GamePage
