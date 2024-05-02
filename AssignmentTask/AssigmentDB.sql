PGDMP  "    5                |            TaskManagerDB    16.1    16.1     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    74066    TaskManagerDB    DATABASE     �   CREATE DATABASE "TaskManagerDB" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United Kingdom.1252';
    DROP DATABASE "TaskManagerDB";
                postgres    false            �            1259    74068    Category    TABLE     \   CREATE TABLE public."Category" (
    "Id" integer NOT NULL,
    "Name" character varying
);
    DROP TABLE public."Category";
       public         heap    postgres    false            �            1259    74067    Category_Id_seq    SEQUENCE     �   ALTER TABLE public."Category" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Category_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    216            �            1259    74076    Task    TABLE       CREATE TABLE public."Task" (
    "Id" integer NOT NULL,
    "Task Name" character varying,
    "Assignee" character varying,
    "CategoryId" integer,
    "Description" character varying,
    "Category" character varying,
    "City" character varying,
    "DueDate" date
);
    DROP TABLE public."Task";
       public         heap    postgres    false            �            1259    74075    Task_Id_seq    SEQUENCE     �   ALTER TABLE public."Task" ALTER COLUMN "Id" ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public."Task_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);
            public          postgres    false    218            �          0    74068    Category 
   TABLE DATA           2   COPY public."Category" ("Id", "Name") FROM stdin;
    public          postgres    false    216   3       �          0    74076    Task 
   TABLE DATA           {   COPY public."Task" ("Id", "Task Name", "Assignee", "CategoryId", "Description", "Category", "City", "DueDate") FROM stdin;
    public          postgres    false    218   �       �           0    0    Category_Id_seq    SEQUENCE SET     @   SELECT pg_catalog.setval('public."Category_Id_seq"', 18, true);
          public          postgres    false    215            �           0    0    Task_Id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public."Task_Id_seq"', 7, true);
          public          postgres    false    217            X           2606    74082    Task TaskManagerCategory_Pk 
   CONSTRAINT     _   ALTER TABLE ONLY public."Task"
    ADD CONSTRAINT "TaskManagerCategory_Pk" PRIMARY KEY ("Id");
 I   ALTER TABLE ONLY public."Task" DROP CONSTRAINT "TaskManagerCategory_Pk";
       public            postgres    false    218            V           2606    74074    Category TaskManager_Pk 
   CONSTRAINT     [   ALTER TABLE ONLY public."Category"
    ADD CONSTRAINT "TaskManager_Pk" PRIMARY KEY ("Id");
 E   ALTER TABLE ONLY public."Category" DROP CONSTRAINT "TaskManager_Pk";
       public            postgres    false    216            Y           2606    74083    Task TaskManagerCategory_FK    FK CONSTRAINT     �   ALTER TABLE ONLY public."Task"
    ADD CONSTRAINT "TaskManagerCategory_FK" FOREIGN KEY ("CategoryId") REFERENCES public."Category"("Id");
 I   ALTER TABLE ONLY public."Task" DROP CONSTRAINT "TaskManagerCategory_FK";
       public          postgres    false    4694    218    216            �   e   x�3���u��2�-H����K�2F0MLS�B3��9T�!d	24@�"���1D��Є3 5/�4��MM�,�U(��/�,��24CȚ#�pf� Q<>      �     x�}��R�@E�=_�? ��p��X(H��ܰi�ƌ�#5�@��;�RSZЛY����d��g\X�+uĜk�<�#i�[��	̭��b���V�����M)�$�DI:��A�r�K>���w.|�ֶ!�N�n �̐:~0R����eɸe._���f�GN��,�':�{�;�!O'ٜ���k��2��r�lqAnǾ�0����5��23x�~qWbM؂wV){�u����j��Y�BE�O���q�$ߜ�p
�u�_�������3<H�%$�j]V�韊�P�	���     