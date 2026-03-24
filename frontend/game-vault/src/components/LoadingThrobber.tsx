function LoadingThrobber() {
    return (
        <main className="max-w-6xl mx-auto px-6 py-8">
            <div className="flex justify-center items-center py-20">
                <div className="w-10 h-10 border-4 border-gray-700 border-t-amber-500 rounded-full animate-spin" />
            </div>
        </main>
    )
}

export default LoadingThrobber
