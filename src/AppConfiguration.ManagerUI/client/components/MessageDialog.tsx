import React from "react";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";

import { withStyles } from "@material-ui/core/styles";

const DialogStyled = withStyles(() => ({
    paper: {
        minWidth: 460,
        borderRadius: 0,
        border: "solid 1px white"
    }
}))(Dialog);

const DialogTitleStyled = withStyles(() => ({
    root: {
        color: "white",
        backgroundColor: "#202020"
    }
}))(DialogTitle);

const DialogContentStyled = withStyles(() => ({
    root: {
        margin: "16px 0",
        padding: 20
    }
}))(DialogContent);

const DialogActionsStyled = withStyles(() => ({
    root: {
        padding: 16,
        backgroundColor: "#202020"
    }
}))(DialogActions);

const ButtonStyled = withStyles(() => ({
    root: {
        color: "black",
        padding: "8px 16px",
        borderRadius: 0,
        minWidth: 148,
        backgroundColor: "#FFC709",
        borderColor: "#FFC709",
        borderWidth: 2,
        '&:hover': {
            backgroundColor: "#E9B60B",
        },
    }
}))(Button)

type MesssageDialogProps = {
    open: boolean;
    title: string;
    message: string;
    onClose: () => void;
}

const messageDialog = ({
    open,
    title,
    message,
    onClose
}: MesssageDialogProps) => (
    <DialogStyled open={open}>
        <DialogTitleStyled>{title}</DialogTitleStyled>
        <DialogContentStyled>{message}</DialogContentStyled>
        <DialogActionsStyled>
            <ButtonStyled onClick={onClose}>OK</ButtonStyled>
        </DialogActionsStyled>
    </DialogStyled>
);

export { messageDialog as MessageDialog };