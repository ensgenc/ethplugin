﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using UnityEngine;

//public node:
//https://ropsten.infura.io
//https://kovan.infura.io
//https://rinkeby.infura.io
//https://mainnet.infura.io
//private node of ganache gui:
//http://127.0.0.1:7545

//etherscan:
//https://ropsten.etherscan.io
//https://kovan.etherscan.io
//https://rinkeby.etherscan.io
//https://etherscan.io

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance = null;

    public string URL;
    public string publicAddress;
    public string privateKey;
    public string password;
    public string encryptedJson;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
    }

    private string ConvertKey(byte[] privateKey)
    {
        // we get the privateKey is byte[] need to convert
        var key = BitConverter.ToString(privateKey);
        key = key.Replace("-", "");
        return key;
    }

    public void CreateAccount(string password)
    {
        var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
        var address = ecKey.GetPublicAddress();
        var privateKey = ecKey.GetPrivateKeyAsBytes();

        var keystoreservice = new Nethereum.KeyStore.KeyStoreService();
        var encryptedJson = keystoreservice.EncryptAndGenerateDefaultKeyStoreAsJson(password, privateKey, address);

        this.password = password;
        this.publicAddress = address;
        this.privateKey = ecKey.GetPrivateKey();
        this.encryptedJson = encryptedJson;
    }

    public void ImportAccountFromJson(string password, string encryptedJson)
    {
        var keystoreservice = new Nethereum.KeyStore.KeyStoreService();

        var privateKey = keystoreservice.DecryptKeyStoreFromJson(password, encryptedJson);

        var address = keystoreservice.GetAddressFromKeyStore(encryptedJson);

        this.password = password;
        this.publicAddress = address;
        this.privateKey = ConvertKey(privateKey);
        this.encryptedJson = encryptedJson;
    }

    public void ImportAccountFromPrivateKey(string key)
    {
        var address = Nethereum.Signer.EthECKey.GetPublicAddress(key);

        this.publicAddress = address;
        this.privateKey = key;
    }

    public TransactionInput GetTransferEtherInput(string data, string toAddress, HexBigInteger gas, HexBigInteger gasPrice, HexBigInteger value)
    {
        TransactionInput input = new TransactionInput(
            data,
            toAddress,
           publicAddress,
            gas, gasPrice, value);
        return input;
    }

    public IEnumerator GetAccountBalance(System.Action<decimal> callback)
    {
        var getBalanceRequest = new EthGetBalanceUnityRequest(URL);
        yield return getBalanceRequest.SendRequest(publicAddress, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());
        if (getBalanceRequest.Exception == null)
        {
            var balance = getBalanceRequest.Result.Value;

            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance));
        }
        else
        {
            throw new System.InvalidOperationException("Get balance request failed");
        }

    }

    // write date to contact ,it  will use some gas
    public IEnumerator SignTransaction(TransactionInput transactionInput, System.Action<UnityRequest<string>> result)
    {
        var transactionSignedRequest = new TransactionSignedUnityRequest(URL, privateKey);

        yield return transactionSignedRequest.SignAndSendTransaction(transactionInput);

        if (result != null)
            result.Invoke(transactionSignedRequest);
    }

    // execute contact function with view keyword or contact field, it doesn't need gas
    public IEnumerator CallTransaction(CallInput callInput, System.Action<UnityRequest<string>> result)
    {
        var callRequest = new EthCallUnityRequest(URL);

        yield return callRequest.SendRequest(callInput, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        if (result != null)
            result.Invoke(callRequest);
    }

}
