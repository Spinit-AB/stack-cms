const path = require('path');

let config = {
    paths: {
        features: path.resolve(__dirname, './Features'),
        dist: path.resolve(__dirname, './dist'),
    }
}

module.exports = {
	context: path.resolve(__dirname, './'),
    entry: path.resolve(__dirname, './Features/angular.module.js'),
    output: {
        path: config.paths.dist,
        filename: 'bundle.js'
    },
    module: {
		rules: [{
                test: /\.js$/,
                exclude: /node_modules/,
                use: [{
                    loader: 'babel-loader?cacheDirectory=true',
                    options: {
                        presets: ['es2015']
                    }
                }]
            }]
    },
	resolve: {
        modules: ['node_modules'],
    },
    devtool: "#inline-source-map"
};