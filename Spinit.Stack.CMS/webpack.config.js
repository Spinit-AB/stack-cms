'use strict'
const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

let config = {
    paths: {
        features: path.resolve(__dirname, './Features'),
        dist: path.resolve(__dirname, './dist'),
        assets: path.resolve(__dirname, './assets/'),
        node_modules: path.resolve(__dirname, './node_modules'),
    }
}

module.exports = {
    context: path.resolve(__dirname, './'),
    entry: {
        main: [
            path.resolve(__dirname, './index.tsx'),
            './assets/styles/scss/app.scss'
        ]
    },
    output: {
        path: config.paths.dist,
        filename: 'scripts/[name].bundle.js',
    },
    plugins: [
        // new webpack.optimize.CommonsChunkPlugin({
        //     name: 'common',
        //     filename: 'scripts/common.js',
        //     minChunks: 2,
        // }),
        new ExtractTextPlugin({
            filename: 'styles/[name].css',
            allChunks: true,
        }),
        // new HappyPack({
        //     id: 'sass',
        //     loaders: ExtractTextPlugin.extract({
        //         fallback: 'style-loader',
        //         use: ['css-loader', 'sass-loader'],
        //     }),
        // }),
        // new webpack.ProvidePlugin({
        //     jQuery: 'jquery',
        //     $: 'jquery',
        //     jquery: 'jquery',
        // }),
    ],
    module: {
        rules: [{
            test: /\.tsx?$/,
            loader: 'ts-loader',
                // options: {
                //     presets: ['es2015', 'react']
                // }
        },
        {
            test: /\.css$/,
            include: config.paths.assets,
            use: ExtractTextPlugin.extract({
                fallback: 'style-loader',
                publicPath: '../',
                use: ['css-loader', 'postcss-loader'],
            }),
        },
        {
            test: /\.(sass|scss)$/,
            include: [
                config.paths.assets,
                config.paths.node_modules,
            ],
            use: ExtractTextPlugin.extract({
                fallback: 'style-loader',
                publicPath: '../',
                use: ['css-loader', 'postcss-loader', 'sass-loader'],
            }),
        },
        {
            test: /\.less$/,
            include: config.paths.assets,
            use: ExtractTextPlugin.extract({
                fallback: 'style-loader',
                publicPath: '../',
                use: ['css-loader', 'less-loader'],
            }),
        },
        {
            test: /\.woff2?(\?v=\d+\.\d+\.\d+)?$/,
            include: [
                config.paths.assets,
                config.paths.node_modules,
            ],
            use: [{
                loader: 'url-loader',
                options: {
                    mimetype: 'application/font-woff',
                },
            }],
        },
        {
            test: /\.(ttf|eot|png|jpe?g|gif|svg|ico)(\?v=\d+\.\d+\.\d+)?$/,
            include: [
                config.paths.assets,
                config.paths.node_modules
            ],
            use: [{
                loader: 'file-loader',
                options: {
                    name: '[path][name].[ext]',
                },
            }],
        },
        ],
    },
    resolve: {
        modules: [config.paths.assets, config.paths.node_modules],
        extensions: ['.ts', '.tsx', '.js']
    },
    devtool: "#inline-source-map"
};