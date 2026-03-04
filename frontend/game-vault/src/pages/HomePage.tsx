import { useState, useEffect } from 'react'
import { getPlatforms } from "../services/api"
import PlatformCard from "../components/PlatformCard"
import LoadingThrobber from "../components/LoadingThrobber"
import ErrorMessage from "../components/ErrorMessage"
import type { Platform } from "../types/game"

function HomePage() {
    const [platforms, setPlatforms] = useState<Platform[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState("")

    useEffect(() => {
        setLoading(true)
        setError("")
        getPlatforms()
            .then(setPlatforms)
            .catch(() => setError("Failed to load platforms"))
            .finally(() => setLoading(false))
    }, [])


    if (loading) return <LoadingThrobber />
    if (error) return <ErrorMessage message={error} onRetry={() => window.location.reload()} />

    return (
        <main className="max-w-6xl mx-auto px-6 py-8">
            <h1 className="text-3xl font-bold mb-8">Browse by Platform</h1>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                {platforms.map((p) => (
                    <PlatformCard key={p.platform} name={p.platform} gameCount={p.game_count} />
                ))}
            </div>
        </main>
    )
}

export default HomePage
