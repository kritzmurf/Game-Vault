import { useEffect, useReducer } from 'react'
import { useParams, Link } from 'react-router-dom'
import { getGameById } from "../services/api"
import type { Game } from "../types/game"
import LoadingThrobber from "../components/LoadingThrobber"
import ErrorMessage from "../components/ErrorMessage"
import { formatPlatformName } from "../utils/formatPlatformName"

interface GameState {
    game: Game | null
    loading: boolean
    error: string
}

type GameAction = 
    | { type: "fetchStart" }
    | { type: "fetchSuccess"; game: Game }
    | { type: "fetchError"; error: string }

const initialState: GameState = {
    game: null,
    loading: true,
    error: "",
}

function gameReducer(state: GameState, action: GameAction): GameState {
    switch (action.type) {
        case "fetchStart":
            return { ...state, loading: true, error: "" }
        case "fetchSuccess":
            return { ...state, loading: false, game: action.game }
        case "fetchError":
            return { ...state, loading: false, error: action.error }
    }
}

function GamePage() {
    const { id } = useParams()
    const [state, dispatch] = useReducer(gameReducer, initialState)

    useEffect(() => {
        if (!id) return
        dispatch({ type: "fetchStart" })
        getGameById(Number(id))
            .then((game) => dispatch({ type: "fetchSuccess", game }))
            .catch(() => dispatch({ type: "fetchError", error: "Failed to load game" }))
    }, [id])

    if (state.loading) return <LoadingThrobber />
    if (state.error) return <ErrorMessage message={state.error} onRetry={() => window.location.reload()} />
    if (!state.game) return null

    return (
        <main className="max-w-4xl mx-auto px-4 sm:px-6 py-8">
            <Link
                to={`/platform/${state.game.platform}`}
                className="text-amber-500 hover:text-amber-400 text-sm"
            >
                ← Back to {formatPlatformName(state.game.platform)}
            </Link>
            <div className="mt-6 flex flex-col sm:flex-row gap-8">
                {state.game.coverArtUrl && (
                    <img
                        src={state.game.coverArtUrl}
                        alt={state.game.title}
                        className="w-48 h-auto rounded-lg shrink-0 self-center sm:self-start"
                    />
                )}
                <div>
                    <h1 className="text-2xl sm:text-3xl font-bold">{state.game.title}</h1>
                    <dl className="mt-4 space-y-2 text-sm">
                        <div>   <dt className="text-gray-500 inline">Developer:</dt>
                                <dd className="inline">{state.game.developer ?? "Unknown"}</dd> 
                        </div>
                        <div>   <dt className="text-gray-500 inline">Publisher:</dt>
                                <dd className="inline">{state.game.publisher ?? "Unknown"}</dd> 
                        </div>
                        <div>   <dt className="text-gray-500 inline">Released:</dt>
                                <dd className="inline">{state.game.releaseDate ?? "Unknown"}</dd> 
                        </div>
                        {/*
                        <div>   <dt className="text-gray-500 inline">Region:</dt>
                                <dd className="inline">{state.game.region ?? "Unknown"}</dd> 
                        </div>
                        */}
                    </dl>
                </div>
            </div>
            {state.game.description && (
                <div className="mt-8">
                    <h2 className="text-xl font-semibold mb-3">Description</h2>
                    <p className="text-gray-300 leading-relaxed">{state.game.description}</p>
                </div>
            )}
        </main>
    )
}

export default GamePage
