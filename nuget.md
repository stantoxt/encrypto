# Encryption Tool 

## Application - Instructions

### 1. Encrypto

This Application is dotnet tool and .net core 2.1 Application.

The main purpose of this tool is to encrypt or decrypt a folder(s) or unique file(s) using AES 256 Encryption Algorithm.
Data will be encrypted with your preferred password.

## Usage

Usage: dotnet-encrypto [params] [options]

** Params:

Parameter Description 
`encrypt` Encrypt Folder(s) or Files
`decrypt` Decrypt Folder(s) or Files

** Options:

Option Description
`--help`   Show help information 
`-i`       Folder or File
`-q`	   Secure Password as input

## dotnet-encrypto tool

* Install tool

`dotnet tool install --global dotnet-encrypto`

* Uninstall tool

`dotnet tool uninstall --global dotnet-encrypto`

## Using tool

`dotnet-encrypto --help`



## Usage:

```
dotnet-encrypto decrypt --help
```

## Encyption

**Encrypt Folder(s) or Files**


```
dotnet-encrypto encrypt -i /home/tcs/Downloads/ -q
```

password as command line parameter:

```
dotnet-encrypto encrypt -i /home/tcs/Downloads/ -p "123"
```

**Encrypt File at Base64 output**

```
dotnet-encrypto encrypt -i /home/tcs/Downloads/file.txt --base64 -p "123" 
```

**Encrypt Text:**

Create a base64 encoded encrypted text:

```
dotnet-encrypto encrypt -t "tom" -q
```

pass password as command line parameter

```
dotnet-encrypto encrypt -t "tom" -p "123"
```

## Decryption

**Decrypt Folder(s) or Files**

```
dotnet-encrypto decrypt -i /home/tcs/Downloads/ -q
```

password as command line parameter

```
dotnet-encrypto decrypt -i /home/tcs/Downloads/ -p "123"
```

**Decrypt File at Base64 output**

```
dotnet-encrypto decrypt -i /home/tcs/Downloads/ --base64 -p "123"
```


#### Decrypt Text:

```
dotnet-encrypto decrypt -t ""P/A6VZEaRHCB8WV9S5m58g==" -q
```
password as command line parameter

```
dotnet-encrypto decrypt -t "P/A6VZEaRHCB8WV9S5m58g==" -p "123"
```
