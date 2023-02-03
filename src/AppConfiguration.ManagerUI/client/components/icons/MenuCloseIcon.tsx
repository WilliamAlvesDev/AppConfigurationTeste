import React from "react";

const menuCloseIcon = ({ size = 16, color = "#FFF" }: any) => {
    const width = size;
    const height = 16 / 11 * size;

    return (
        <svg width={width} height={height} viewBox="0 0 16 11" xmlns="http://www.w3.org/2000/svg">
            <path fill={color} fill-rule="evenodd" d="M16 10v1H6v-1h10zm0-5v1H0V5h16zm0-5v1H6V0h10z" />
        </svg>
    );
};

export { menuCloseIcon as MenuCloseIcon };